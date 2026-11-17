# Secure local configuration

Do not commit `appsettings*.json`, mail credentials, database credentials, or other secrets. `LinkUpPro/appsettings.example.json` is a safe template only; copy it locally to `appsettings.Development.json` when needed. That file is ignored by Git.

For local development, use .NET User Secrets from the `LinkUpPro` project directory. Store `ConnectionStrings:DefaultConnection` and `SmtpSettings:Password` there; SMTP server and account settings can also be supplied as secrets or environment variables. Environment-variable names use double underscores, such as `ConnectionStrings__DefaultConnection` and `SmtpSettings__Password`.

For deployment, inject values through the hosting platform's secret store. Rotate any SMTP or database credential that has ever been committed.
