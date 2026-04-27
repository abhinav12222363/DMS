using Dms.Application.Abstractions;
using Dms.Application.Auth;
using Dms.Domain.Entities;

namespace Dms.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ICaptchaValidator _captchaValidator;
    private readonly IPasswordResetTokenStore _passwordResetTokenStore;
    private readonly IEmailSender _emailSender;

    public AuthService(
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        ICaptchaValidator captchaValidator,
        IPasswordResetTokenStore passwordResetTokenStore,
        IEmailSender emailSender)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _captchaValidator = captchaValidator;
        _passwordResetTokenStore = passwordResetTokenStore;
        _emailSender = emailSender;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var captchaOk = await _captchaValidator.ValidateAsync(request.CaptchaToken, ct);
        if (!captchaOk)
        {
            throw new UnauthorizedAccessException("CAPTCHA validation failed.");
        }

        var user = await _userRepository.GetByUsernameAsync(request.Username, ct)
            ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!user.IsActive || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var expiresAt = DateTime.UtcNow.AddHours(8);
        var token = _jwtTokenGenerator.GenerateToken(user, expiresAt);
        return new LoginResponse(token, expiresAt, user.Username, user.Role);
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, ct);
        if (user is null)
        {
            return;
        }

        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        await _passwordResetTokenStore.StoreAsync(user.Id, token, ct);
        await _emailSender.SendAsync(user.Email, "DMS Password Reset", $"Use this token to reset your password: {token}", ct);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct)
    {
        var userId = await _passwordResetTokenStore.ConsumeAsync(request.Token, ct)
            ?? throw new UnauthorizedAccessException("Invalid or expired token.");

        var user = await _userRepository.GetByIdAsync(userId, ct)
            ?? throw new KeyNotFoundException("User not found.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _userRepository.SaveChangesAsync(ct);
    }
}
