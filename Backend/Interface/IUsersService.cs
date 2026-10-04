

using System.Security.Claims;

public interface IUsersService
{
     Task<ServiceResult<UpdateUserResponse>> UpdateAsync(UpdateUserRequest request, ClaimsPrincipal claimsPrincipal);
}