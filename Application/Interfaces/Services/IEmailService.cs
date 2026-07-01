using LinkUpProject.Application.DTOs.Email;

namespace Application.Interfaces.Services;

public interface IEmailService
{
    Task SendAsync(EmailRequest request);
}