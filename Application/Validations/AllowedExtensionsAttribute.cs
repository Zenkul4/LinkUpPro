using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LinkUpProject.Application.Validations;

public class AllowedExtensionsAttribute : ValidationAttribute
{
    private readonly string[] _extensions;

    public AllowedExtensionsAttribute(string[] extensions)
    {
        _extensions = extensions;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(extension) || !_extensions.Contains(extension.ToLower()))
            {
                return new ValidationResult($"El archivo seleccionado no tiene un formato válido. Formatos permitidos: {string.Join(", ", _extensions)}");
            }
        }

        return ValidationResult.Success;
    }
}