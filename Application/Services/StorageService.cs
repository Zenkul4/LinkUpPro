using LinkUpProject.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace LinkUpProject.Application.Services;

public class StorageService : IStorageService
{
    private readonly IWebHostEnvironment _env;

    public StorageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> UploadFileAsync(IFormFile file, string directoryName)
    {
        var basePath = Path.Combine(_env.WebRootPath, "images", directoryName);

        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
        }

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var fullPath = Path.Combine(basePath, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/images/{directoryName}/{fileName}";
    }

    public void DeleteFile(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl)) return;

        var relativePath = fileUrl.TrimStart('/');
        var fullPath = Path.Combine(_env.WebRootPath, relativePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}