


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApiProject.Migrations;

public class RecurringRulesService(UserManager<ApplicationUser> _userManager, AppDbContext _context) : IRecurringRulesService
{
    public async Task<ServiceResult<CreateRecurringRuleResponse>> CreateAsync(CreateRecurringRuleRequest request, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<CreateRecurringRuleResponse>.Fail("User not found");
        }

        var recurringRule = new RecurringRule
        {
            UserId = user.Id,
            AccountId = request.AccountId,
            Amount = request.Amount,
            CategoryId = request.CategoryId,
            Frequency = request.Frequency,
            NextRunDate = request.NextRunDate,
            IsActive = true
        };

        _context.RecurringRules.Add(recurringRule);
        await _context.SaveChangesAsync();

        var auditLog = new AuditLog
        {
            EntityType = "RecurringRules",
            EntityId = recurringRule.Id,
            UserId = user.Id,
            Action = "Created",
            OldValue = "N/A",
            NewValue = $"Account: {recurringRule.AccountId}, Amount: {recurringRule.Amount}, Category: {recurringRule.CategoryId} Frequency: {recurringRule.Frequency}, Next run Date: {recurringRule.NextRunDate}",
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);

        await _context.SaveChangesAsync();

        var response = new CreateRecurringRuleResponse(
            Id: recurringRule.Id,
            AccountId: recurringRule.AccountId,
            Amount: recurringRule.Amount,
            CategoryId: recurringRule.CategoryId,
            Frequency: recurringRule.Frequency,
            NextRunDate: recurringRule.NextRunDate
        );

        return ServiceResult<CreateRecurringRuleResponse>.Ok(response);

    }

    public async Task<ServiceResult<List<RecurringRule>>> GetAllAsync(ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<List<RecurringRule>>.Fail("User not found");
        }

        var AllrecurringRules = await _context.RecurringRules
        .Where(r => r.UserId == user.Id && r.IsDeleted == false)
        .Include(r => r.Account)
        .Include(r => r.Category)
        .ToListAsync();

        if (AllrecurringRules is null)
        {
            return ServiceResult<List<RecurringRule>>.Fail("No recurring rules found");
        }

        return ServiceResult<List<RecurringRule>>.Ok(AllrecurringRules);
    }

    public async Task<ServiceResult<UpdateRecurringRuleResponse>> UpdateAsync(int ruleId, UpdateRecurringRuleRequest request, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<UpdateRecurringRuleResponse>.Fail("User not found");
        }

        var recurringRule = await _context.RecurringRules.FirstOrDefaultAsync(r => r.Id == ruleId && r.IsDeleted == false);
        if (recurringRule is null)
        {
            return ServiceResult<UpdateRecurringRuleResponse>.Fail("Recurring rule not found");
        }

        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == recurringRule.AccountId && a.IsDeleted == false);
        if (account is null)
        {
            return ServiceResult<UpdateRecurringRuleResponse>.Fail("Account not found");
        }

        if (account.UserId != user.Id)
        {
            return ServiceResult<UpdateRecurringRuleResponse>.Fail("You do now own this account");
        }

        var oldAccountId = recurringRule.AccountId;
        var oldAmount = recurringRule.Amount;
        var oldCategoryId = recurringRule.CategoryId;
        var oldFrequency = recurringRule.Frequency;
        var oldNextRunDate = recurringRule.NextRunDate;

        recurringRule.AccountId = request.AccountId;
        recurringRule.Amount = request.Amount;
        recurringRule.CategoryId = request.CategoryId;
        recurringRule.Frequency = request.Frequency;
        recurringRule.NextRunDate = request.NextRunDate;

        var auditLog = new AuditLog
        {
            EntityType = "RecurringRules",
            EntityId = recurringRule.Id,
            UserId = user.Id,
            Action = "Updated",
            OldValue = $"Account: {oldAccountId}, Amount: {oldAmount}, Category: {oldCategoryId}, Frequency: {oldFrequency}, Next run date: {oldNextRunDate}",
            NewValue = $"account: {recurringRule.AccountId}, Amount: {recurringRule.Amount}, Category: {recurringRule.CategoryId}, Frequeny: {recurringRule.Frequency}, Next run date: {recurringRule.NextRunDate}",
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();

        var response = new UpdateRecurringRuleResponse(
            Id: recurringRule.Id,
            AccountId: recurringRule.AccountId,
            Amount: recurringRule.Amount,
            CategoryId: recurringRule.CategoryId,
            Frequency: recurringRule.Frequency,
            NextRunDate: recurringRule.NextRunDate
        );

        return ServiceResult<UpdateRecurringRuleResponse>.Ok(response);


    }

    public async Task<ServiceResult<bool>> DeleteAsync(int ruleId, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<bool>.Fail("User not found");
        }

        var recurringRule = await _context.RecurringRules.FirstOrDefaultAsync(r => r.Id == ruleId && r.IsDeleted == false);
        if (recurringRule is null)
        {
            return ServiceResult<bool>.Fail("Recurring rule not found");
        }

        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == recurringRule.AccountId && a.IsDeleted == false);
        if (account is null)
        {
            return ServiceResult<bool>.Fail("Account not found");
        }

        if (account.UserId != user.Id)
        {
            return ServiceResult<bool>.Fail("You do not own this account");
        }

        recurringRule.IsDeleted = true;

        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }
}