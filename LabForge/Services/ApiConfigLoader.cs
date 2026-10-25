using Microsoft.Extensions.Configuration;

namespace LabForge.Services;
public sealed class ApiConfigLoader
{
    private const string FileName = "api.json";
    private const string EnvPrefix = "LABFORGE_";

    public ApiConfig Load()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(FileName, optional: true, reloadOnChange: false)
            .AddEnvironmentVariables(EnvPrefix)
            .Build();

        IConfigurationSection section = configuration.GetSection(ApiConfig.SectionName);

        string apiKey = section["ApiKey"]?.Trim() ?? string.Empty;
        string model = section["Model"]?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException(
                $"Gemini API key не найден. Укажите \"{ApiConfig.SectionName}:ApiKey\" " +
                $"в файле {FileName} (рядом с приложением) " +
                $"или в переменной окружения {EnvPrefix}{ApiConfig.SectionName}__ApiKey.");
        }

        return new ApiConfig
        {
            ApiKey = apiKey,
            Model = string.IsNullOrEmpty(model) ? ApiConfig.DefaultModel : model
        };
    }
}