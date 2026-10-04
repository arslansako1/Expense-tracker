


using Microsoft.EntityFrameworkCore;
using MyApiProject.Migrations;

public class AccountService(AppDbContext _context) : IAccountService
{
    public async Task<ServiceResult<CreateAccResponse>> CreateAsync(string userId, CreateAccRequest request)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return ServiceResult<CreateAccResponse>.Fail("User not found");
        }

        if (string.IsNullOrEmpty(request.Name))
        {
            return ServiceResult<CreateAccResponse>.Fail("Name cant be empty");
        }

        var account = new Account
        {
            UserId = userId,
            Name = request.Name,
            Type = request.Type,
            Currency = request.Currency,
            Balance = 0,
            CreatedAt = DateTime.UtcNow
        };

        _context.Accounts.Add(account);

        var auditLog = new AuditLog
        {
            EntityType = "Accounts",
            EntityId = account.Id,
            UserId = userId,
            Action = "Created",
            OldValue = "N/A",
            NewValue = $"Name: {account.Name}, Type: {account.Type}, Currency: {account.Currency}, Balance: {account.Balance}",
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();

        var response = new CreateAccResponse(
            Id: account.Id,
            Name: account.Name,
            Type: account.Type,
            Currency: account.Currency,
            Balance: account.Balance,
            CreatedAt: account.CreatedAt
        );

        return ServiceResult<CreateAccResponse>.Ok(response);



    }
    public async Task<ServiceResult<List<Account>>> GetAllAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return ServiceResult<List<Account>>.Fail("User not found");

        }

        var accounts = await _context.Accounts
     .Where(a => a.UserId == userId && a.IsDeleted == false).OrderByDescending(a => a.CreatedAt).ToListAsync();

        if (accounts is null)
        {
            return ServiceResult<List<Account>>.Fail("No accounts in this user");
        }

        return ServiceResult<List<Account>>.Ok(accounts);

    }
    public async Task<ServiceResult<UpdateAccResponse>> UpdateAsync(string userId, int accountId, UpdateAccRequest request)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return ServiceResult<UpdateAccResponse>.Fail("User not found");
        }

        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId && a.IsDeleted == false);
        if (account is null)
        {
            return ServiceResult<UpdateAccResponse>.Fail("Account not found");
        }

        if (account.UserId != userId)
        {
            return ServiceResult<UpdateAccResponse>.Fail("You dont own this account");
        }

        var oldName = account.Name;
        var oldType = account.Type;
        var oldCurrency = account.Currency;
        var oldBalance = account.Balance;

        account?.Name = request.Name;
        account?.Type = request.Type;
        account?.Currency = request.Currency;

        var auditLog = new AuditLog
        {
            EntityType = "Accounts",
            EntityId = account!.Id,
            UserId = userId,
            Action = "Updated",
            OldValue = $"Name: {oldName}, Type: {oldType}, Currency: {oldCurrency}, Balance: {oldBalance}",
            NewValue = $"Name: {account.Name}, Type: {account.Type}, Currency: {account.Currency}, Balance: {account.Balance}",
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);

        await _context.SaveChangesAsync();

        var response = new UpdateAccResponse(
             Id: account.Id,
            Name: account.Name,
            Type: account.Type,
            Currency: account.Currency,
            Balance: account.Balance,
            UpdatedAt: DateTime.UtcNow
        );

        return ServiceResult<UpdateAccResponse>.Ok(response);



    }
    public async Task<ServiceResult<bool>> DeleteAsync(string userId, int accountId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return ServiceResult<bool>.Fail("User not found");
        }

        var account = await _context.Accounts.FindAsync(accountId);
        if (account is null)
        {
            return ServiceResult<bool>.Fail("Account not found");
        }

        if (account.UserId != userId)
        {
            return ServiceResult<bool>.Fail("You dont own this account");
        }

        account.IsDeleted = true;


        var auditLog = new AuditLog
        {
            EntityType = "Accounts",
            EntityId = account.Id,
            UserId = userId,
            Action = "Deleted",
            OldValue = $"Name: {account.Name}, Type: {account.Type}, Currency: {account.Currency}, Balance: {account.Balance}",
            NewValue = "N/A",
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.NoContent("Account deleted successfully");


    }
    public async Task<ServiceResult<ProjectedBalanceResponse>> GetProjectedBalanceViewAsync(string userId, int accountId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return ServiceResult<ProjectedBalanceResponse>.Fail("User not found");
        }


        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId && a.IsDeleted == false);
        if (account is null)
        {
            return ServiceResult<ProjectedBalanceResponse>.Fail("Account not found");
        }
        if (account.UserId != userId)
        {
            return ServiceResult<ProjectedBalanceResponse>.Fail("You do not own this account");
        }

        var allRecurringRules = await _context.RecurringRules
        .Where(r => r.AccountId == accountId && r.IsDeleted == false).ToListAsync();

        if (allRecurringRules is null)
        {
            return ServiceResult<ProjectedBalanceResponse>.Ok(new ProjectedBalanceResponse(
                CurrentBalance: account.Balance,
                Projections: new List<ProjectionItem>()
            ));
        }

        var currentBalance = account.Balance;
        var projections = new List<ProjectionItem>();

        foreach (var rule in allRecurringRules)
        {
            var currentDate = DateTime.UtcNow.Date;

            var endDate = currentDate.AddMonths(3);

            while (currentDate <= endDate)
            {
                if (rule.NextRunDate.Date == currentDate)
                {
                    var projection = new ProjectionItem
                    (
                        Date: currentDate,
                        Description: $"Recurring: {rule.Frequency}",
                        Amount: -rule.Amount,
                        BalanceAfter: currentBalance - rule.Amount
                    );

                    projections.Add(projection);
                    currentBalance -= rule.Amount;

                    if (rule.Frequency == "Weekly")
                        rule.NextRunDate = rule.NextRunDate.AddDays(7);
                    else if (rule.Frequency == "Monthly")
                        rule.NextRunDate = rule.NextRunDate.AddMonths(1);
                    else if (rule.Frequency == "Yearly")
                        rule.NextRunDate = rule.NextRunDate.AddYears(1);
                }

                currentDate = currentDate.AddDays(1);
            }
        }

        return ServiceResult<ProjectedBalanceResponse>.Ok(new ProjectedBalanceResponse(
           CurrentBalance: account.Balance,
           Projections: projections.OrderBy(p => p.Date).ToList()
       ));


    }
}