using InclusiveCode.API.Models;
using InclusiveCode.API.Services;
using InclusiveCode.API.Data;
using InclusiveCode.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace InclusiveCode.API.Controllers
{
    [ApiController]
    [Route("api/analyze")]
    public class RepositoryController : ControllerBase
    {
        private readonly PythonAnalyzerService _pythonService;
        private readonly AppDbContext _context;

        public RepositoryController(PythonAnalyzerService pythonService, AppDbContext context)
        {
            _pythonService = pythonService;
            _context = context;
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

            // Calculate score before saving and sending
            var issuesList = result.Issues ?? new List<PythonAnalyzerIssue>();
            var fileGroups = new Dictionary<string, int>();

            foreach (var item in issuesList)
            {
                var filename = string.IsNullOrWhiteSpace(item.Filename) ? "arquivo-desconhecido" : item.Filename;
                var lineValue = item.Line.HasValue && item.Line.Value > 0 ? item.Line.Value : 0;

                if (fileGroups.TryGetValue(filename, out var currentMax))
                {
                    fileGroups[filename] = Math.Max(currentMax, lineValue);
                }
                else
                {
                    fileGroups[filename] = lineValue;
                }
            }

            int fileCount = Math.Max(fileGroups.Count, 1);
            int estimatedTotalLines = fileGroups.Values.Sum(maxLine => maxLine > 0 ? Math.Max(maxLine, 80) : 120);

            int issuesCount = issuesList.Count;
            double divisor = Math.Max((double)estimatedTotalLines / 100.0, 1.0);
            double issuesPer100Lines = issuesCount / divisor;
            double issuesPerFile = (double)issuesCount / fileCount;

            double penalty = (issuesPer100Lines * 6.0) + (issuesPerFile * 12.0) + (Math.Max(0, issuesCount - 2) * 2.0);

            int score = (int)Math.Round(Math.Clamp(100.0 - penalty, 0, 100));

            string label;
            if (score >= 85) label = "INCLUSIVO";
            else if (score >= 70) label = "BOA INCLUSAO";
            else if (score >= 50) label = "FALTA INCLUSAO";
            else if (score >= 30) label = "POUCO INCLUSIVO";
            else label = "NADA INCLUSIVO";

            result.Score = score;
            result.ScoreLabel = label;

            // Save analysis result for history
            try
            {
                var raw = JsonSerializer.Serialize(result);

                var analysis = new AnalysisResult
                {
                    RepoUrl = request.Url ?? string.Empty,
                    RawJson = raw,
                    Score = score,
                    ScoreLabel = label
                };

                // Use user id provided by front-end when available (check UserId or Id)
                if (request.UserId.HasValue)
                {
                    analysis.UserId = request.UserId.Value;
                }
                else if (request.Id.HasValue)
                {
                    analysis.UserId = request.Id.Value;
                }
                else if (!string.IsNullOrEmpty(request.Email))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                    if (user != null)
                    {
                        analysis.UserId = user.Id;
                    }
                }

                _context.AnalysisResults.Add(analysis);
                await _context.SaveChangesAsync();
            }
            catch
            {
                // Don't fail the request if saving history fails; just continue.
            }

            return Ok(result);
        }

        [HttpGet("history/me")]
        public async Task<IActionResult> GetMyHistory([FromQuery] int userId)
        {
            Console.WriteLine($"[DEBUG] GetMyHistory called with userId: {userId}");

            // Public endpoint where front sends userId as query parameter
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            Console.WriteLine($"[DEBUG] User exists: {userExists}");

            if (!userExists)
                return NotFound(new { message = "Usuário não encontrado." });

            var items = await _context.AnalysisResults
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new InclusiveCode.API.Models.AnalysisResultDto
                {
                    Id = a.Id,
                    RepoUrl = a.RepoUrl,
                    RawJson = a.RawJson,
                    Score = a.Score,
                    ScoreLabel = a.ScoreLabel,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            Console.WriteLine($"[DEBUG] Found {items.Count} analysis results for user {userId}");

            return Ok(items);
        }
    }
}
