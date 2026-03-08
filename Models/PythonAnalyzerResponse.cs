using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace InclusiveCode.API.Models
{
    public class PythonAnalyzerResponse
    {
        [JsonPropertyName("summary")]
        public string Summary { get; set; } = string.Empty;

        [JsonPropertyName("non_conforming_count")]
        public int NonConformingCount { get; set; }

        [JsonPropertyName("issues")]
        public List<PythonAnalyzerIssue> Issues { get; set; } = new();

        // Raw JSON returned by the analyzer. Not part of the mapped response model.
        [JsonIgnore]
        public string? RawResponse { get; set; }

        // Optional top-level status from the analyzer wrapper (e.g. "success").
        public string? Status { get; set; }
    }
}
