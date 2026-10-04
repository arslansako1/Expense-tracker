using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;


[ApiController]
[Route("api/accounts")]
[Authorize]
public class AccountsController(IAccountService _iAccountService, UserManager<ApplicationUser> _userManager) : ControllerBase
{

    [HttpPost("createAccount")]
    public async Task<ActionResult> CreateAccountAsync(CreateAccRequest request)
    {
        var user  = await _userManager.GetUserAsync(User);
        if (user is null)
        {
           return Unauthorized("User not found");
        }

        var result = await _iAccountService.CreateAsync(user.Id, request);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }
    
    [HttpGet("getAllAccounts")]
    public async Task<ActionResult> GetAllAccountsAsync()
    {
        var user  = await _userManager.GetUserAsync(User);
        if (user is null)
        {
           return Unauthorized("User not found");
        }

        var result = await _iAccountService.GetAllAsync(user.Id);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }
   

    [HttpPut("updateAccount/{accountId}")]
    public async Task<ActionResult> UpdateAccountAsync(int accountId, UpdateAccRequest request)
    {
        var user  = await _userManager.GetUserAsync(User);
        if (user is null)
        {
           return Unauthorized("User not found");
        }

        var result = await _iAccountService.UpdateAsync(user.Id, accountId, request);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }


    [HttpDelete("deleteAccount/{accountId}")]
    public async Task<ActionResult> DeleteAccountAsync(int accountId)
    {
        var user  = await _userManager.GetUserAsync(User);
        if (user is null)
        {
           return Unauthorized("User not found");
        }

        var result = await _iAccountService.DeleteAsync(user.Id, accountId);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return NoContent();    
    }

    [HttpGet("projectedbalanceView/{accountId}")]
    public async Task<ActionResult> ProjectedBalanceViewAsync(int accountId)
    {
        var user  = await _userManager.GetUserAsync(User);
        if (user is null)
        {
           return Unauthorized("User not found");
        }

        var result = await _iAccountService.GetProjectedBalanceViewAsync(user.Id, accountId);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    } 

}