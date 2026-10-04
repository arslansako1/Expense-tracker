

using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkyCast.API.Services;

public class AuthService(AppDbContext _context, UserManager<ApplicationUser> _userManager, TokenService _tokenService, EmailService _emailService) : IAuthService
{
    public async Task<ServiceResult<SignupResponse>> SignupAsync(SignupRequest request)

    { if (await _userManager.FindByEmailAsync(request.Email!) is not null)
        {
            return ServiceResult<SignupResponse>.Fail("Email is already taken.");
        }

        var user = new ApplicationUser
        {
            FirstName = request.FirstName!,
            LastName = request.LastName!,
            Email = request.Email,
            UserName = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password!);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return ServiceResult<SignupResponse>.Fail(errors);
        }


        var response = new SignupResponse(
            Message: $"User '{request.Email}' signed up successfully",
            Email: request.Email!
        );

        return ServiceResult<SignupResponse>.Ok(response);


        
    }
    public async Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request)
    {
          var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return ServiceResult<AuthResponse>.Fail("Password's do not match");
        }


        var (token, expiresAt) =  _tokenService.CreateToken(user);

        var refreshToken = _tokenService.CreateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
        };

        await _context.RefreshTokens.AddAsync(refreshTokenEntity);
        await _context.SaveChangesAsync();

        var response = new AuthResponse(
            AccessToken: token,
            RefreshToken: refreshToken,
            ExpiresAt: expiresAt,
            UserId: user.Id,
            Email: user.Email!,
            FirstName: user.FirstName,
            LastName: user.LastName
        );


        return ServiceResult<AuthResponse>.Ok(response);
        
        
    }
    public async Task<ServiceResult<ForgetPasswordResponse>> ForgetPassword(ForgetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return ServiceResult<ForgetPasswordResponse>.Fail("If your email exists, you'll receive a reset link.");    
        };

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var encodedToken = WebUtility.UrlEncode(token);
        var encodedEmail = WebUtility.UrlEncode(request.Email);

        var resetLink = $"http://localhost:5173/resetPassword?token={encodedToken}&email={encodedEmail}";

  
        var emailBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 10px;'>
                <h1 style='color: #1a1a2e;'>🔐 Reset Your Password</h1>
                <p>Hello,</p>
                <p>We received a request to reset your password for SpendWise.</p>
                <p style='margin: 30px 0;'>
                    <a href='{resetLink}' style='background: #fbbf24; color: #1a1a2e; padding: 14px 28px; border-radius: 8px; text-decoration: none; font-weight: 600;'>
                        🔗 Reset Password
                    </a>
                </p>
                <p style='color: #666; font-size: 14px;'>This link will expire in <strong>1 hour</strong>.</p>
                <hr style='border: none; border-top: 1px solid #eee;'/>
                <p style='color: #999; font-size: 12px;'>If you didn't request this, please ignore this email.</p>
                <p style='color: #999; font-size: 12px;'>© 2026 SpendWise</p>
            </div>
        ";

        await _emailService.SendEmailAsync(request.Email, "Reset your skyCast password", emailBody);

        var response = new ForgetPasswordResponse(
            Message: "Reset link sent to your password"
        );

        return ServiceResult<ForgetPasswordResponse>.Ok(response);
        

        
    }
    public async Task<ServiceResult<ResetPasswordResponse>> ResetPassword(ResetPasswordRequest request)
    {
       var decodedToken = request.Token;
        var decodedEmail = request.Email;

           if (!string.IsNullOrEmpty(request.Token))
    {
        try
        {
            decodedToken = Uri.UnescapeDataString(request.Token);
        }
        catch
        {
            decodedToken = WebUtility.UrlDecode(request.Token);
        }
    }


        var user = await _userManager.FindByEmailAsync(decodedEmail);
        if (user == null)
        {
            return ServiceResult<ResetPasswordResponse>.Fail("Invalid request." );
        };

        var result = await _userManager.ResetPasswordAsync(user, decodedToken!, request.NewPassword!);

        if (!result.Succeeded)
        {
            return ServiceResult<ResetPasswordResponse>.Fail("Reset failed");

     
        }
        
        return ServiceResult<ResetPasswordResponse>.Ok("Password reset successfully");

     
        
    }
    public async Task<ServiceResult<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest reqeust)
    {
         var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == reqeust.RefreshToken);

        if (refreshToken is null)
        {
            throw new BadRequestException("Invalid refresh token");
        }

        if (refreshToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new BadRequestException("Refresh token has expired");
        }

        var user = await _userManager.FindByIdAsync(refreshToken.UserId);

        if (user is null)
        {
            throw new BadRequestException("User not found");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var (newAcessToken, expiresAt) =  _tokenService.CreateToken(user);

        var newRefreshToken = _tokenService.CreateRefreshToken();

        _context.RefreshTokens.Remove(refreshToken);

        var refreshTokenEntity = new RefreshToken{

            Token = newRefreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
        };

        await _context.RefreshTokens.AddAsync(refreshTokenEntity);
    

        await _context.SaveChangesAsync();

        var resposne = new AuthResponse(
            AccessToken: newAcessToken,
            RefreshToken: newRefreshToken,
            ExpiresAt: expiresAt,
            UserId: user.Id,
            Email: user.Email!,
            FirstName: user.FirstName,
            LastName: user.LastName

        );

        return ServiceResult<AuthResponse>.Ok(resposne);
     
    }
}