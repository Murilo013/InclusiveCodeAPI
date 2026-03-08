using InclusiveCode.API.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace InclusiveCode.API.Services
{
    public class PythonAnalyzerService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PythonAnalyzerService> _logger;

        public PythonAnalyzerService(HttpClient httpClient, ILogger<PythonAnalyzerService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<PythonAnalyzerResponse?> RunAnalysis(string repoUrl)
        {
            var body = new { url = repoUrl };

            var content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PostAsync("analyze", content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Python analyzer");
                return new PythonAnalyzerResponse { RawResponse = ex.Message };
            }

            var responseString = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Python analyzer returned status {StatusCode} and body: {Body}", response.StatusCode, responseString);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            try
            {
                // The analyzer sometimes returns a wrapper with top-level 'status' and 'analysis' object.
                using var doc = JsonDocument.Parse(responseString);
                JsonElement root = doc.RootElement;

                string? topStatus = null;
                if (root.TryGetProperty("status", out var statusProp) && statusProp.ValueKind == JsonValueKind.String)
                {
                    topStatus = statusProp.GetString();
                }

                JsonElement analysisElement;
                if (root.TryGetProperty("analysis", out analysisElement))
                {
                    var analysisJson = analysisElement.GetRawText();
                    var result = JsonSerializer.Deserialize<PythonAnalyzerResponse>(analysisJson, options);
                    if (result == null)
                    {
                        return new PythonAnalyzerResponse { RawResponse = responseString };
                    }

                    result.RawResponse = responseString;
                    if (!string.IsNullOrEmpty(topStatus)) result.Status = topStatus;
                    return result;
                }

                // Fallback: try to deserialize the root directly into the model.
                var fallback = JsonSerializer.Deserialize<PythonAnalyzerResponse>(responseString, options);
                if (fallback != null)
                {
                    fallback.RawResponse = responseString;
                    if (!string.IsNullOrEmpty(topStatus)) fallback.Status = topStatus;
                    return fallback;
                }

                return new PythonAnalyzerResponse { RawResponse = responseString };
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Failed to deserialize Python analyzer response");
                return new PythonAnalyzerResponse { RawResponse = responseString };
            }
        }
    }
}
