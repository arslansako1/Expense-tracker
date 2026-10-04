

using System.Security.Claims;

public interface ITransferService
{
    Task<ServiceResult<CreateTransferResponse>> CreateAsync(CreateTransferRequest request, ClaimsPrincipal claimsPrincipal);
    Task<ServiceResult<List<Transfer>>> GetAllAsync(ClaimsPrincipal claimsPrincipal);
    Task<ServiceResult<List<Transfer>>> GetAllAccAsync(int accountId, ClaimsPrincipal claimsPrincipal);

}
    
