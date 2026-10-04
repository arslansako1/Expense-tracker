

using System.Security.Claims;

public interface ICsvFileUploadService
{
    Task<ServiceResult<UploadFileResponse>> UploadAsync(IFormFile file, ClaimsPrincipal claimsPrincipal);
    Task<ServiceResult<ConfirmImportResponse>> ConfirmAsync(ConfirmImportRequest request, ClaimsPrincipal claimsPrincipal);
}