


public interface IAuthService
{
    Task<ServiceResult<SignupResponse>> SignupAsync(SignupRequest request);
    Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request);
    Task<ServiceResult<ForgetPasswordResponse>> ForgetPassword(ForgetPasswordRequest request);
    Task<ServiceResult<ResetPasswordResponse>> ResetPassword(ResetPasswordRequest request);
    Task<ServiceResult<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest reqeust);
}