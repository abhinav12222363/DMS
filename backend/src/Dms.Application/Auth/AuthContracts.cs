using Dms.Domain.Enums;

namespace Dms.Application.Auth;

public sealed record LoginRequest(string Username, string Password, string CaptchaToken);
public sealed record LoginResponse(string Token, DateTime ExpiresAtUtc, string Username, UserRole Role);
public sealed record ForgotPasswordRequest(string Email);
public sealed record ResetPasswordRequest(string Token, string NewPassword);
