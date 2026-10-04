


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApiProject.Migrations;

public class UsersService(UserManager<ApplicationUser> _userManager) : IUsersService
{
    public async Task<ServiceResult<UpdateUserResponse>> UpdateAsync(UpdateUserRequest request, ClaimsPrincipal claimsPrincipal)
    {
    var userId1 = claimsPrincipal.FindFirstValue("sub");
    var userId2 = claimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.Sub);
    var userId3 = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
 
    var userId = userId1 ?? userId2 ?? userId3;
    
    if (string.IsNullOrEmpty(userId))
    {
        return ServiceResult<UpdateUserResponse>.Fail("User ID not found in token.");
    }
       
        if (string.IsNullOrEmpty(userId))
        {
            return ServiceResult<UpdateUserResponse>.Fail("User ID not found in token.");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if(user is null)
        {
            return ServiceResult<UpdateUserResponse>.Fail("User not found");
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
      

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return ServiceResult<UpdateUserResponse>.Fail("Update failed");
        }

        var response = new UpdateUserResponse(
            Id: user.Id,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Email: user.Email
        );


        return ServiceResult<UpdateUserResponse>.Ok(response);
  
        
    }
}