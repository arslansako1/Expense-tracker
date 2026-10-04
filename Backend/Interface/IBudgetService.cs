

using System.Security.Claims;

public interface IBudgetService
{
    Task<ServiceResult<CreateBudgetResponse>> CreateAsync(CreateBudgetRequest request, ClaimsPrincipal claimsPrincipal);
    Task<ServiceResult<List<Budget>>> GetAllAsync(ClaimsPrincipal claimsPrincipal);
    Task<ServiceResult<bool>> DeleteAsync(int budgetId, ClaimsPrincipal claimsPrincipal);
    Task<ServiceResult<UpdateBudgetResponse>> UpdateAsync(int budgetId, UpdateBudgetRequest request, ClaimsPrincipal claimsPrincipal);
}
    
