namespace LabForge.Services;


public sealed class ApiConfig
{
    public const string SectionName = "Gemini";
    public const string DefaultModel = "gemini-2.5-flash";

    public string ApiKey { get; init; } = string.Empty;

    public string Model { get; init; } = DefaultModel;
}