using InclusiveCode.API.Models;
using InclusiveCode.API.Services;
using Microsoft.AspNetCore.Mvc;

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

            return Ok(result);
        }
    }
}
