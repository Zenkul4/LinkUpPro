using LinkUpProject.Application.DTOs;

namespace LinkUpProject.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendAsync(EmailRequest request);
}
