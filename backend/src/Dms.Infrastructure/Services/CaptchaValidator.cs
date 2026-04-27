using Dms.Application.Abstractions;

namespace Dms.Infrastructure.Services;

public sealed class CaptchaValidator : ICaptchaValidator
{
    public Task<bool> ValidateAsync(string token, CancellationToken ct)
    {
        // Replace with Google reCAPTCHA/Turnstile verifier in production.
        return Task.FromResult(!string.IsNullOrWhiteSpace(token) && token.Length > 5);
    }
}
