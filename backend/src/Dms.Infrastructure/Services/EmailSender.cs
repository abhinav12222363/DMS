using Dms.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace Dms.Infrastructure.Services;

public sealed class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(ILogger<EmailSender> logger) => _logger = logger;

    public Task SendAsync(string toEmail, string subject, string body, CancellationToken ct)
    {
        _logger.LogInformation("Email queued to {To}; subject: {Subject}; body: {Body}", toEmail, subject, body);
        return Task.CompletedTask;
    }
}
