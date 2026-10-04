

using System.Security.Claims;

public interface IReportsService
{
    Task<ServiceResult<List<GetCategorieSpendResponse>>> GetSpendAsync(ClaimsPrincipal claimsPrincipal);
    Task<ServiceResult<List<GetBalanceHistoryResponse>>> GetBalanceAsync(ClaimsPrincipal claimsPrincipal, int months = 1);
    Task<ServiceResult<GetIncomeExpenseResponse>> GetIncomeAsync(ClaimsPrincipal claimsPrincipal);

}
    
