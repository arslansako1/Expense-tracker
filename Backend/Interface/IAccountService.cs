


public interface IAccountService
{
    Task<ServiceResult<CreateAccResponse>> CreateAsync(string userId, CreateAccRequest request);
    Task<ServiceResult<List<Account>>> GetAllAsync(string userId);
    Task<ServiceResult<UpdateAccResponse>> UpdateAsync(string userId, int accountId, UpdateAccRequest request);
    Task<ServiceResult<bool>> DeleteAsync(string userId, int accountId);
    Task<ServiceResult<ProjectedBalanceResponse>> GetProjectedBalanceViewAsync(string userId, int accountId);
}