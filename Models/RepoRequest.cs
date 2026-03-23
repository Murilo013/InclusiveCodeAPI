namespace InclusiveCode.API.Models
{
    public class RepoRequest
    {
        public string Url { get; set; } = string.Empty;

        // Optional: email of the user requesting analysis. If provided, saved with the result.
        public string? Email { get; set; }

        // Optional: user id sent by the front-end to associate the analysis
        public int? UserId { get; set; }

        // Alias for UserId (if front sends "id" instead of "userId")
        public int? Id { get; set; }
    }
}
