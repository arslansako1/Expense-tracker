

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(IUsersService _service) : ControllerBase
{
    
    [HttpPut("updateMe")]
    public async Task<ActionResult> UpdateMeAsync(UpdateUserRequest request)
    {
        var result = await _service.UpdateAsync(request, User);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }
        
        return Ok(result.Data);
    }
}