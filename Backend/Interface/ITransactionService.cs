

using System.Security.Claims;

public interface ITransactionsService
{
    Task<ServiceResult<CreateTransactionResponse>> CreateAsync(CreateTransactionRequest request, ClaimsPrincipal claimsPrincipal);
    Task<ServiceResult<List<Transaction>>> GetAllAsync(ClaimsPrincipal claimsPrincipal);
    Task<ServiceResult<List<Transaction>>> GetAllAccAsync(int accountId, ClaimsPrincipal claimsPrincipal);
    Task<ServiceResult<bool>> DeleteAsync(int transactionId, ClaimsPrincipal claimsPrincipal);
    Task<ServiceResult<UpdateTransactionResponse>> UpdateAsync(int transactionId, UpdateTransactionRequest request, ClaimsPrincipal claimsPrincipal);
    Task<ServiceResult<bool>> CheckBudget(string userId, int? categoryId);
}
    
