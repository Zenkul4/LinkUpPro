using Microsoft.AspNetCore.Http;

namespace LinkUpProject.Application.Interfaces.Services;

public interface IStorageService
{
    Task<string> UploadFileAsync(IFormFile file, string directoryName);
    void DeleteFile(string fileUrl);
}