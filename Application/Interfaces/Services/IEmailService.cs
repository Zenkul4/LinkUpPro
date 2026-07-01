using LinkUpProject.Application.DTOs;

namespace Application.Interfaces.Services;

public interface IEmailService
{
    Task SendAsync(EmailRequest request);
}
