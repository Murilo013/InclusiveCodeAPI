namespace InclusiveCode.API.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        
        // Em um cenário real, as senhas NUNCA devem ser armazenadas em texto plano.
        // Utilize o BCrypt ou ASP.NET Core Identity para fazer o hash.
        public string PasswordHash { get; set; } = string.Empty;
    }
}