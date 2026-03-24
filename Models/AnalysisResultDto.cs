using System;

namespace InclusiveCode.API.Models
{
    public class AnalysisResultDto
    {
        public int Id { get; set; }
        public string RepoUrl { get; set; } = string.Empty;
        public string RawJson { get; set; } = string.Empty;
        public int Score { get; set; }
        public string ScoreLabel { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
