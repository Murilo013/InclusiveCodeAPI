using InclusiveCode.API.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace InclusiveCode.API.Services
{
    public class PythonAnalyzerService
    {
        private readonly HttpClient _httpClient;

        public PythonAnalyzerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PythonAnalyzerResponse?> RunAnalysis(string repoUrl)
        {
            var body = new { url = repoUrl };

            var content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("analyze", content);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseString = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<PythonAnalyzerResponse>(responseString, options);
        }
    }
}
