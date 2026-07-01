using LinkUpProject.Application.DTOs.Email;

namespace LinkUpProject.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendAsync(EmailRequest request);
}
