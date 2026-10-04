


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApiProject.Migrations;

public class TransferService(UserManager<ApplicationUser> _userManager, AppDbContext _context) : ITransferService
{
    public async Task<ServiceResult<CreateTransferResponse>> CreateAsync(CreateTransferRequest request, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<CreateTransferResponse>.Fail("User not found");
        }

        var fromAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == request.FromAccountId && a.IsDeleted == false);
        if (fromAccount is null)
        {
            return ServiceResult<CreateTransferResponse>.Fail("Account not found");
        }

        var toAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == request.ToAccountId && a.IsDeleted == false);
        if (toAccount is null)
        {
            return ServiceResult<CreateTransferResponse>.Fail("Account not found");
        }

        if (fromAccount.UserId != user.Id)
        {
            return ServiceResult<CreateTransferResponse>.Fail("You don't own the source account");
        }

        if (toAccount.UserId != user.Id)
        {
            return ServiceResult<CreateTransferResponse>.Fail("You don't own the destination account");
        }

        if (request.FromAccountId == request.ToAccountId)
        {
            return ServiceResult<CreateTransferResponse>.Fail("Cannot transfer to the same account");
        }

        if (fromAccount.Balance < request.Amount)
        {
            return ServiceResult<CreateTransferResponse>.Fail("Not enough balance");
        }

        var tranfer = new Transfer
        {
            UserId = user.Id,
            FromAccountId = request.FromAccountId,
            ToAccountId = request.ToAccountId,
            Amount = request.Amount,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            Date = request.Date

        };


        var transaction1 = new Transaction
        {
            UserId = user.Id,
            AccountId = request.FromAccountId,
            Amount = -request.Amount,
            Description = request.Description,
            Type = "Transfer",
            Date = request.Date,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };


        var transaction2 = new Transaction
        {
            UserId = user.Id,
            AccountId = request.ToAccountId,
            Amount = request.Amount,
            Description = request.Description,
            Type = "Transfer",
            Date = request.Date,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        _context.Transfers.Add(tranfer);

        _context.Transactions.Add(transaction1);
        _context.Transactions.Add(transaction2);

        fromAccount.Balance -= request.Amount;
        toAccount.Balance += request.Amount;

        var auditLog = new AuditLog
        {
            EntityType = "Transfers",
            EntityId = tranfer.Id,
            UserId = user.Id,
            Action = "Created",
            OldValue = "N/A",
            NewValue = $"From Account: {tranfer.FromAccountId}, To Account: {tranfer.ToAccountId}, Amount: {tranfer.Amount}, Description: {tranfer.Description}",
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);

        try
        {
            await _context.SaveChangesAsync();

        }
        catch (DbUpdateConcurrencyException)
        {
            return ServiceResult<CreateTransferResponse>.Fail("This account was updated by someone else. please refresh and try again");
        }

        var response = new CreateTransferResponse(
            Id: tranfer.Id,
            Amount: tranfer.Amount,
            Description: tranfer.Description,
            FromAccountId: tranfer.FromAccountId,
            ToAccountId: tranfer.ToAccountId,
            Date: tranfer.Date,
            CreatedAt: tranfer.CreatedAt
        );

        return ServiceResult<CreateTransferResponse>.Ok(response);
    }

    public async Task<ServiceResult<List<Transfer>>> GetAllAsync(ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<List<Transfer>>.Fail("User not found");
        }

        var allTransfers = await _context.Transfers
        .Where(t => t.UserId == user.Id && t.IsDeleted == false)
        .Include(f => f.FromAccount)
        .Include(t => t.ToAccount)
        .OrderByDescending(t => t.Date)
        .ToListAsync();

        if (!allTransfers.Any())
        {
            return ServiceResult<List<Transfer>>.Ok(allTransfers);
        }

        return ServiceResult<List<Transfer>>.Ok(allTransfers);
    }

    public async Task<ServiceResult<List<Transfer>>> GetAllAccAsync(int accountId, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<List<Transfer>>.Fail("User not found");
        }

        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId && a.IsDeleted == false);
        if (account is null)
        {
            return ServiceResult<List<Transfer>>.Fail("Account not found");
        }

        if (account.UserId != user.Id)
        {
            return ServiceResult<List<Transfer>>.Fail("You do not own this account");
        }

        var allTransfers = await _context.Transfers
        .Where(t => (t.FromAccountId == accountId || t.ToAccountId == accountId) && t.IsDeleted == false)
        .OrderByDescending(o => o.Date)
        .Include(t => t.FromAccount)
        .Include(t => t.ToAccount)
        .ToListAsync();

        if (!allTransfers.Any())
        {
            return ServiceResult<List<Transfer>>.Ok(allTransfers);
        }



        return ServiceResult<List<Transfer>>.Ok(allTransfers); ;

    }

}