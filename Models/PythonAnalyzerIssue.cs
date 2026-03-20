using System.Text.Json.Serialization;

namespace InclusiveCode.API.Models
{
    public class PythonAnalyzerIssue
    {
        [JsonPropertyName("filename")]
        public string Filename { get; set; } = string.Empty;

        [JsonPropertyName("line")]
        public int? Line { get; set; }

        [JsonPropertyName("snippet")]
        public string Snippet { get; set; } = string.Empty;

        [JsonPropertyName("issue")]
        public string Issue { get; set; } = string.Empty;

        [JsonPropertyName("severity")]
        public string Severity { get; set; } = string.Empty;

        [JsonPropertyName("improvement")]
        public string Improvement { get; set; } = string.Empty;

        [JsonPropertyName("evidence_urls")]
        public List<string> EvidenceUrls { get; set; } = new();
    }
}
