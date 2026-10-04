


using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApiProject.Migrations;

public class CsvFileUploadService(AppDbContext _context, UserManager<ApplicationUser> _userManager) : ICsvFileUploadService
{
    public async Task<ServiceResult<UploadFileResponse>> UploadAsync(IFormFile file, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<UploadFileResponse>.Fail("User not found");
        }

        if (file is null && file?.Length == 0)
        {
            return ServiceResult<UploadFileResponse>.Fail("No file uploaded");
        }

        if (!file!.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return ServiceResult<UploadFileResponse>.Fail("File must be a CSV file");
        }

        var validTransaction = new List<TransactionPreviewResponse>();
        var errors = new List<ImportErrorsResponse>();
        var rowNumber = 0;

        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            string ?line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                            
                rowNumber++;

                if (rowNumber == 1) continue;

                var columns = line.Split(",");

                if (columns.Length < 3)
                {
                    errors.Add(new ImportErrorsResponse(
                        RowNumbers: rowNumber,
                        Errors: "Row must have at least 3 columns (Date, Description, Amount)",
                        RowData: line
                    ));
                    continue;
                }

                var date = DateTime.UtcNow;
                if (!DateTime.TryParse(columns[0], out date))
                {
                    errors.Add(new ImportErrorsResponse(
                        RowNumbers: rowNumber,
                        Errors: $"Invalid data format: '{columns[0]}'. Use format: dd/mm/yyyy",
                        RowData: line
                    ));
                    continue;
                }

                var Description = columns[1].Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(Description))
                {
                    errors.Add(new ImportErrorsResponse(
                    RowNumbers: rowNumber,
                    Errors: $"Description is required",
                    RowData: line
                    ));
                    continue;
                }

                var amount = 0m;
                if (!decimal.TryParse(columns[2], out amount))
                {
                    errors.Add(new ImportErrorsResponse(
                        RowNumbers: rowNumber,
                        Errors: $"Invalid amount format: '{columns[2]}'. Use format: 10.00 or -10.00",
                        RowData: line
                    ));
                    continue;
                }

                 date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
                
                validTransaction.Add(new TransactionPreviewResponse
                (
                    Date: date,
                    Description: Description,
                    Amount: amount
                ));
            }

            var response = new UploadFileResponse(
                TotalRows: rowNumber - 1,
                SuccessCount: validTransaction.Count,
                ErrorCount: errors.Count,
                ValidTransactions: validTransaction,
                Errors: errors
            );

            return ServiceResult<UploadFileResponse>.Ok(response);
        }  
    
    }

    public async Task<ServiceResult<ConfirmImportResponse>> ConfirmAsync(ConfirmImportRequest request, ClaimsPrincipal claimsPrincipal)
    { 
       
    var user = await _userManager.GetUserAsync(claimsPrincipal);
    if (user is null)
    {
        return ServiceResult<ConfirmImportResponse>.Fail("User not found");
    }

    if (request.Transactions == null || request.Transactions.Count == 0)
    {
        return ServiceResult<ConfirmImportResponse>.Fail("No transactions to import");
    }

    var account = await _context.Accounts.FindAsync(request.AccountId);
    if (account is null)
    {
        return ServiceResult<ConfirmImportResponse>.Fail("Account not found");
    }

    if (account.UserId != user.Id)
    {
        return ServiceResult<ConfirmImportResponse>.Fail("You don't own this account");
    }

    var importedCount = 0;
    var skippedCount = 0;
    var errors = new List<string>();

    using var dbTransaction = await _context.Database.BeginTransactionAsync();

    try
    {
        foreach (var item in request.Transactions)
        {
            var isDuplicate = await _context.Transactions
                .AnyAsync(t => t.UserId == user.Id
                    && t.AccountId == request.AccountId
                    && t.Date.Date == item.Date.Date
                    && t.Description == item.Description
                    && t.Amount == Math.Abs(item.Amount)
                    && !t.IsDeleted);

            if (isDuplicate)
            {
                skippedCount++;
                errors.Add($"Duplicate: {item.Description} on {item.Date:yyyy-MM-dd}");
                continue;
            }

            var type = item.Amount >= 0 ? "Income" : "Expense";
            var amount = Math.Abs(item.Amount);

            var utcDate = DateTime.SpecifyKind(item.Date, DateTimeKind.Utc);

            var transaction = new Transaction
            {
                AccountId = request.AccountId,
                UserId = user.Id,
                Amount = amount,
                Type = type,
                Description = item.Description,
                CategoryId = request.CategoryId ?? 0,
                Date = utcDate,
                CreatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);

            if (type == "Income")
            {
                account.Balance += amount;
            }
            else
            {
                if (account.Balance < amount)
                {
                    errors.Add($"Insufficient balance for: {item.Description} (${amount})");
                    continue;  
                }
                account.Balance -= amount;
            }

            importedCount++;
        }

        var auditLog = new AuditLog
        {
            EntityType = "CSVImport",
            EntityId = 0,
            UserId = user.Id,
            Action = "Imported",
            OldValue = "N/A",
            NewValue = $"Imported {importedCount} transactions. Skipped {skippedCount} duplicates.",
            Timestamp = DateTime.UtcNow
        };
        _context.AuditLogs.Add(auditLog);

        await _context.SaveChangesAsync();
        await dbTransaction.CommitAsync();

        var response =  new ConfirmImportResponse(
            ImportedCount: importedCount,
            SkippedCount: skippedCount,
            Errors: errors
        );


        return ServiceResult<ConfirmImportResponse>.Ok(response);
    }
    catch (Exception ex)
    {
        await dbTransaction.RollbackAsync();
        return ServiceResult<ConfirmImportResponse>.Fail($"Import failed: {ex.Message}");
    }
        
    }
}