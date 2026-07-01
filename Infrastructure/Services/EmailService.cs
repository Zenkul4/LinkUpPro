using LinkUpProject.Application.DTOs;
using LinkUpProject.Application.DTOs.Email;
using LinkUpProject.Application.Interfaces.Services;
using LinkUpProject.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace LinkUpProject.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;

    public EmailService(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }

    public async Task SendAsync(EmailRequest request)
    {
        using var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port);

        client.UseDefaultCredentials = false;

        client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
        client.EnableSsl = _smtpSettings.EnableSsl;

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
            Subject = request.Subject,
            Body = request.Body,
            IsBodyHtml = request.IsHtml
        };

        mailMessage.To.Add(request.To);

        await client.SendMailAsync(mailMessage);
    }
}