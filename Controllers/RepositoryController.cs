using InclusiveCode.API.Models;
using InclusiveCode.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace InclusiveCode.API.Controllers
{
    [ApiController]
    [Route("api/analyze")]
    public class RepositoryController : ControllerBase
    {
        private readonly PythonAnalyzerService _pythonService;

        public RepositoryController(PythonAnalyzerService pythonService)
        {
            _pythonService = pythonService;
        }

        [HttpPost]
        public async Task<IActionResult> Analyze([FromBody] RepoRequest request)
        {
            var result = await _pythonService.RunAnalysis(request.Url);

            if (result == null)
                return StatusCode(500, "Erro ao processar resposta do Python.");

            if (!string.IsNullOrEmpty(result.RawResponse))
            {
                try
                {
                    using var doc = JsonDocument.Parse(result.RawResponse);
                    var root = doc.RootElement;
                    if (root.TryGetProperty("status", out var statusProp) &&
                        statusProp.ValueKind == JsonValueKind.String &&
                        string.Equals(statusProp.GetString(), "error", StringComparison.OrdinalIgnoreCase))
                    {
                        return new ContentResult
                        {
                            StatusCode = 502,
                            Content = result.RawResponse,
                            ContentType = "application/json"
                        };
                    }
                }
                catch (JsonException)
                {
                    // ignore parse errors and fall back to returning the parsed result
                }
            }

            return Ok(result);
        }
    }
}
