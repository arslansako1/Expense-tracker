


using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApiProject.Migrations;

public class BudgetService(AppDbContext _context, UserManager<ApplicationUser> _userManager) : IBudgetService
{
    public async Task<ServiceResult<CreateBudgetResponse>> CreateAsync(CreateBudgetRequest request, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<CreateBudgetResponse>.Fail("User not found");
        }

        var budget = new Budget
        {
            UserId = user.Id,
            CategoryId = request.CategoryId,
            MonthlyLimit = request.MonthlyLimit,
            Month = request.Month,
            CreatedAt = DateTime.UtcNow
        };

        _context.Budgets.Add(budget);

        var auditLog = new AuditLog
        {
            EntityType = "Budgets",
            EntityId = budget.Id,
            UserId = user.Id,
            Action = "Created",
            OldValue = "N/A",
            NewValue = $"Monthly limit: {budget.MonthlyLimit}, Category: {budget.CategoryId}",
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);

        await _context.SaveChangesAsync();

        var response = new CreateBudgetResponse(
            Id: budget.Id,
            CategoryId: budget.CategoryId,
            Month: budget.Month,
            MonthlyLimit: budget.MonthlyLimit,
            CreatedAt:  budget.CreatedAt
        );


        return ServiceResult<CreateBudgetResponse>.Ok(response);
        
    }

    public async Task<ServiceResult<List<Budget>>> GetAllAsync(ClaimsPrincipal claimsPrincipal)
    { 
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<List<Budget>>.Fail("User not found");
        }

        var budgets = await _context.Budgets
        .Where(b => b.UserId == user.Id && b.IsDeleted == false)
        .Include(b => b.Category)
        .ToListAsync();
        
        if (budgets is null)
        {
            return ServiceResult<List<Budget>>.Fail("No budgets found");
        }


        return ServiceResult<List<Budget>>.Ok(budgets);
        
    }
    public async Task<ServiceResult<bool>> DeleteAsync(int budgetId, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<bool>.Fail("User not found");
        }

        var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.Id == budgetId && b.IsDeleted == false);
        if (budget is null)
        {
            return ServiceResult<bool>.Fail("Budget not found");
        }

        if (budget.UserId != user.Id)
        {
            return ServiceResult<bool>.Fail("You do not own this account");
        }

        budget.IsDeleted = true;

         var auditLog = new AuditLog
        {
            EntityType = "Budgets",
            EntityId = budget.Id,
            UserId = user.Id,
            Action = "Deleted",
            OldValue = $"Monthly limit: {budget.MonthlyLimit}, Category: {budget.CategoryId}",
            NewValue = "N/A",
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);

        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
        
    }
    public async Task<ServiceResult<UpdateBudgetResponse>> UpdateAsync(int budgetId, UpdateBudgetRequest request, ClaimsPrincipal claimsPrincipal)
    { 
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return ServiceResult<UpdateBudgetResponse>.Fail("User not found");
        }
        
           var budget = await _context.Budgets.FindAsync(budgetId);
        if (budget is null)
        {
            return ServiceResult<UpdateBudgetResponse>.Fail("Budget not found");
        }

        if (budget.UserId != user.Id)
        {
            return ServiceResult<UpdateBudgetResponse>.Fail("You do not own this account");
        }

        var oldCategory = budget.CategoryId;
        var oldMonthlyLimit = budget.MonthlyLimit;

        budget.Month = request.Month;
        budget.CategoryId = request.CategoryId;
        budget.MonthlyLimit = request.MonthlyLimit;

         var auditLog = new AuditLog
        {
            EntityType = "Budgets",
            EntityId = budget.Id,
            UserId = user.Id,
            Action = "Updated",
            OldValue = $"Monthly limit: {oldMonthlyLimit}, Category: {oldCategory}",
            NewValue = $"Monthly limit: {budget.MonthlyLimit}, Category: {budget.CategoryId}",
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);

        await _context.SaveChangesAsync();

        var response = new UpdateBudgetResponse(
            Id: budget.Id,
            MonthlyLimit: budget.MonthlyLimit,
            CategoryId: budget.CategoryId,
            Month: budget.Month,
            CreatedAt: budget.CreatedAt
        );

        return ServiceResult<UpdateBudgetResponse>.Ok(response);
    
    }
}