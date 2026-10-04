

using System.Security.Claims;

public interface IRecurringRulesService
{
    Task<ServiceResult<CreateRecurringRuleResponse>> CreateAsync(CreateRecurringRuleRequest request, ClaimsPrincipal claimsPrincipal);
    Task<ServiceResult<List<RecurringRule>>> GetAllAsync(ClaimsPrincipal claimsPrincipal);
    Task<ServiceResult<bool>> DeleteAsync(int ruleId, ClaimsPrincipal claimsPrincipal);
    Task<ServiceResult<UpdateRecurringRuleResponse>> UpdateAsync(int ruleId, UpdateRecurringRuleRequest request, ClaimsPrincipal claimsPrincipal);

}
    
