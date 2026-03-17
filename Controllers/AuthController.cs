using InclusiveCode.API.Data;
using InclusiveCode.API.Entities;
using InclusiveCode.API.Models;
using InclusiveCode.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InclusiveCode.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;

        public AuthController(AppDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Verifica se o usuário já existe
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new { message = "E-mail já está em uso." });
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                // TODO: Adicionar um Hash de senha aqui (Ex: BCrypt.Net) antes de salvar
                PasswordHash = request.Password 
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuário registrado com sucesso!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            // TODO: Aqui a senha recebida deve ser comparada com o hash gerado
            if (user == null || user.PasswordHash != request.Password)
            {
                return Unauthorized(new { message = "Credenciais inválidas." });
            }

            // TODO: Em um cenário real, retorne um Token JWT aqui
            return Ok(new { message = "Login realizado com sucesso!", username = user.Username });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            // Validar se as senhas novas conferem
            if (request.NewPassword != request.NewPasswordConfirmation)
            {
                return BadRequest(new { message = "A nova senha e a confirmação não conferem." });
            }

            // Validar se a senha nova é diferente da antiga
            if (request.OldPassword == request.NewPassword)
            {
                return BadRequest(new { message = "A nova senha não pode ser igual à antiga." });
            }

            // Buscar o usuário pelo email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado." });
            }

            // Validar senha antiga (TODO: comparar com hash BCrypt quando implementado)
            if (user.PasswordHash != request.OldPassword)
            {
                return Unauthorized(new { message = "Senha antiga incorreta." });
            }

            // Atualizar a senha (TODO: fazer hash BCrypt aqui)
            user.PasswordHash = request.NewPassword;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Senha atualizada com sucesso!" });
        }
    }
}