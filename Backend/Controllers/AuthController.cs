using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using SkyCast.API.Services;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService _authService) : ControllerBase
{

    [HttpPost("signup")]
    public async Task<ActionResult> SignupAsync(SignupRequest request)
    {

        var result = await _authService.SignupAsync(request);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }
    
        return Ok(result.Data);
    }

    [HttpPost("login")]
    public async Task<ActionResult> LoginAsync(LoginRequest request)
    {

       var result = await _authService.LoginAsync(request);
       if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    [HttpPost("forgetPassword")]
    public async Task<ActionResult>  ForgetPassword(ForgetPasswordRequest request)
    {
         var result = await _authService.ForgetPassword(request);
       if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }


    [HttpPost("resetPassword")]
    public async Task<ActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var result = await _authService.ResetPassword(request);
       if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
        
    }

    [HttpPost("refreshToken")]
    public async Task<ActionResult> RefreshTokenAsync(RefreshTokenRequest request)
    {

        var result = await _authService.RefreshTokenAsync(request);
       if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }
}