using Google.GenAI;
using Google.GenAI.Types;

namespace LabForge.Services
{
    public class GeminiService
    {
        private const string DefaultModel = "gemini-2.5-flash";

        private readonly Client _client;
        private readonly string _model;

        public GeminiService(string apiKey, string model = DefaultModel)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("API key must not be empty.", nameof(apiKey));

            _client = new Client(apiKey: apiKey);
            _model = model;
        }

        public async Task<string> GenerateTextAsync(string prompt, int maxTokens = 1024)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("Prompt must not be empty.", nameof(prompt));

            var response = await _client.Models.GenerateContentAsync(
                model: _model,
                contents: prompt,
                config: new GenerateContentConfig
                {
                    MaxOutputTokens = maxTokens
                });

            return response.Text ?? string.Empty;
        }
    }
}