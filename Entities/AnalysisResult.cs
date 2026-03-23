using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace InclusiveCode.API.Entities
{
    public class AnalysisResult
    {
        public int Id { get; set; }

        // Optional relation to User
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public string RepoUrl { get; set; } = string.Empty;

        // Raw JSON returned by the analyzer
        public string RawJson { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
