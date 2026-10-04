


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApiProject.Migrations;

public class TransactionService(UserManager<ApplicationUser> _userManager, AppDbContext _context) : ITransactionsService
{
    public async Task<ServiceResult<CreateTransactionResponse>> CreateAsync(CreateTransactionRequest request, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<CreateTransactionResponse>.Fail("User is not found");
        }

        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == request.AccountId && a.IsDeleted == false);
        if (account is null)
        {
            return ServiceResult<CreateTransactionResponse>.Fail("Account not found");
        }

        if (account.UserId != user.Id)
        {
            return ServiceResult<CreateTransactionResponse>.Fail("You don't own this account");
        }


        if (request.Type != "Income" && request.Type != "Expense")
        {
            return ServiceResult<CreateTransactionResponse>.Fail("Type must be 'Income' or 'Expense'");
        }

        if (request.Amount <= 0)
        {
            return ServiceResult<CreateTransactionResponse>.Fail("Amount must be greater than 0");
        }

        if (request.Type == "Expense" && account.Balance < request.Amount)
        {
            return ServiceResult<CreateTransactionResponse>.Fail($"Not enough balance");
        }

        if (request.Type == "Income")
        {
            account.Balance += request.Amount;
        }
        else
        {
            account.Balance -= request.Amount;
        }

        var transaction = new Transaction
        {
            AccountId = request.AccountId,
            UserId = user.Id,
            Amount = request.Amount,
            Type = request.Type,
            Description = request.Description,
            CategoryId = request.CategoryId,
            Date = request.Date,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Transactions.AddAsync(transaction);

        var auditLog = new AuditLog
        {
            EntityType = "Transactions",
            EntityId = transaction.Id,
            UserId = user.Id,
            Action = "Created",
            OldValue = "N/A",
            NewValue = $"account: {transaction.AccountId}, Amount: {transaction.Amount}, Category: {transaction.CategoryId} Description: {transaction.Description}",
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);

        try
        {
            await _context.SaveChangesAsync();

        }
        catch (DbUpdateConcurrencyException)
        {
            return ServiceResult<CreateTransactionResponse>.Fail("This account was updated by someone else. please refresh and try again");
        }

        await CheckBudget(user.Id, transaction.CategoryId);

        var response = new CreateTransactionResponse(
            Id: transaction.Id,
            AccountId: transaction.AccountId,
            Amount: transaction.Amount,
            Type: transaction.Type,
            Description: transaction.Description,
            CreatedAt: transaction.CreatedAt,
            Date: transaction.Date,
            CategoryId: transaction.CategoryId
        );

        return ServiceResult<CreateTransactionResponse>.Ok(response);
    }

    public async Task<ServiceResult<List<Transaction>>> GetAllAsync(ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<List<Transaction>>.Fail("User not found");
        }

        var transactions = await _context.Transactions
        .Where(t => t.UserId == user.Id && t.IsDeleted == false)
        .Include(t => t.Category)
        .Include(a => a.Account)
        .OrderByDescending(t => t.Date)
        .ToListAsync();

        if (transactions is null)
        {
            return ServiceResult<List<Transaction>>.Fail("Account not found");
        }

        return ServiceResult<List<Transaction>>.Ok(transactions);

    }

    public async Task<ServiceResult<List<Transaction>>> GetAllAccAsync(int accountId, ClaimsPrincipal claimsPrincipal)
    {
       var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<List<Transaction>>.Fail("User not found");
        }

        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId && a.IsDeleted == false);
        if (account is null)
        {
            return ServiceResult<List<Transaction>>.Fail("Account not found");
        }

        if (account.UserId != user.Id)
        {
            return ServiceResult<List<Transaction>>.Fail("You dont own this account");    
        }

        var transactions = await _context.Transactions
        .Where(t => t.AccountId == accountId && t.IsDeleted == false)
        .Include(t => t.Category)
        .OrderByDescending(t => t.Date)
        .ToListAsync();

        return ServiceResult<List<Transaction>>.Ok(transactions);

    }

    public async Task<ServiceResult<UpdateTransactionResponse>> UpdateAsync(int transactionId, UpdateTransactionRequest request, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<UpdateTransactionResponse>.Fail("User not found");
        }


        var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == transactionId && t.IsDeleted == false);
        if (transaction is null)
        {
            return ServiceResult<UpdateTransactionResponse>.Fail("Transaction not found");
        }

        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == transaction.AccountId && a.IsDeleted == false);
        if (account is null)
        {
            return ServiceResult<UpdateTransactionResponse>.Fail("Account not found");
        }


        if (account.UserId != user.Id)
        {
            return ServiceResult<UpdateTransactionResponse>.Fail("You dont own this account");
        }

        var balanceWithoutOldTransaction = account.Balance;
    
    if (transaction.Type == "Income")
    {
        balanceWithoutOldTransaction -= transaction.Amount;
    }
    else if (transaction.Type == "Expense")
    {
        balanceWithoutOldTransaction += transaction.Amount;
    }

    if (request.Type == "Expense" && balanceWithoutOldTransaction < request.Amount)
    {
        return ServiceResult<UpdateTransactionResponse>.Fail($"Not enough balance");
    }

    if (transaction.Type == "Income")
    {
        account.Balance -= transaction.Amount;
    }
    else if (transaction.Type == "Expense")
    {
        account.Balance += transaction.Amount;
    }



        var oldAccountId = transaction.AccountId;
        var oldAmount = transaction.Amount;
        var oldCategoryId = transaction.CategoryId;
        var oldDescription = transaction.Description;

        transaction.Amount = request.Amount;
        transaction.CategoryId = request.CategoryId;
        transaction.Type = request.Type;
        transaction.Description = request.Description;

        account.Balance += transaction.Amount;

        var auditLog = new AuditLog
        {
            EntityType = "Transactions",
            EntityId = transaction.Id,
            UserId = user.Id,
            Action = "Updated",
            OldValue = $"Account: {oldAccountId}, Amount: {oldAmount}, Category: {oldCategoryId}, Description: {oldDescription}",
            NewValue = $"account: {transaction.AccountId}, Amount: {transaction.Amount}, Category: {transaction.CategoryId} Description: {transaction.Description}",
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);
        

        try
        {
          await _context.SaveChangesAsync();   

        }
        catch (DbUpdateConcurrencyException)
        {
            return ServiceResult<UpdateTransactionResponse>.Fail("This account was updated by someone else. please refresh and try again");
        }

        await CheckBudget(user.Id, transaction.CategoryId);

        var response = new UpdateTransactionResponse(
            Id: transaction.Id,
            AccountId: transaction.AccountId,
            Amount: transaction.Amount,
            Type: transaction.Type,
            Description: transaction.Description,
            CreatedAt: transaction.CreatedAt,
            Date: transaction.Date,
            CategoryId: transaction.CategoryId
        );

        return ServiceResult<UpdateTransactionResponse>.Ok(response);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int transactionId, ClaimsPrincipal claimsPrincipal)
    {
       var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<bool>.Fail("User not found");
        }

        var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == transactionId && t.IsDeleted == false);
        if (transaction is null)
        {
            return ServiceResult<bool>.Fail("Transaction does not exist");
        }

        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == transaction.AccountId && a.IsDeleted == false);
        if (account is null)
        {
            return ServiceResult<bool>.Fail("Account does not exist");
        }

        if (account.UserId != user.Id)
        {
            return ServiceResult<bool>.Fail("You dont own this account");
        }

        transaction.IsDeleted = true;

        if (transaction.Type == "Income")
        {
          account.Balance -= transaction.Amount;
        } 

        else if (transaction.Type == "Expense")
        {
            account.Balance += transaction.Amount;
        }


         var auditLog = new AuditLog{
            EntityType = "Transactions",
            EntityId = transaction.Id,
            UserId = user.Id,
            Action = "Deleted",
            OldValue = $"account: {transaction.AccountId}, Amount: {transaction.Amount}, Category: {transaction.CategoryId} Description: {transaction.Description}",
            NewValue = "N/A",
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);

        try
        {
          await _context.SaveChangesAsync();   

        }
        catch (DbUpdateConcurrencyException)
        {
            return ServiceResult<bool>.Fail("This account was updated by someone else. please refresh and try again");
        }

        await CheckBudget(user.Id, transaction.CategoryId);

        return ServiceResult<bool>.Ok(true);
  
    }

    public async Task<ServiceResult<bool>> CheckBudget(string userId, int? categoryId)

    {
       var budget = await _context.Budgets
        .FirstOrDefaultAsync(b => b.CategoryId == categoryId&& b.UserId == userId && b.IsDeleted == false);

        if (budget is null)
        {
            return ServiceResult<bool>.Ok(true);
        }

        var totalSpent = await _context.Transactions.Where(t => t.CategoryId == categoryId
         && t.UserId == userId
         && t.Date.Year == DateTime.UtcNow.Year
         && t.Date.Month == DateTime.UtcNow.Month
         && t.IsDeleted == false)
         .SumAsync(t => t.Amount);

           if (totalSpent > budget!.MonthlyLimit)
    {
   
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId && c.IsDeleted == false);
            
            var notification = new Notification
            {
                UserId = userId,
                Message = $"⚠️ Budget exceeded! You've spent {totalSpent:C} of your {budget.MonthlyLimit:C} budget for {category?.Name ?? "this category"}.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

        
    }
            return ServiceResult<bool>.Ok(true);

    }
}