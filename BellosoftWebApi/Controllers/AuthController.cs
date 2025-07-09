using BellosoftWebApi.Core;
using BellosoftWebApi.Models;
using BellosoftWebApi.Requests;
using BellosoftWebApi.Responses;
using BellosoftWebApi.Services;
using BellosoftWebApi.Services.AuthenticatedUser;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security;
using System.Security.Claims;
using BCryptNet = BCrypt.Net.BCrypt;

namespace BellosoftWebApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext context;

        public AuthController(AppDbContext context) : base()
        {
            this.context = context;
        }

        [HttpPost("signup")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Signup([FromBody]LoginRequest request)
        {
            await HttpContext.SignOutAsync("Cookies");

            string email = request.Email;
            string passwordHash = BCryptNet.HashPassword(request.Password);

            bool emailExists = await context.Users.AnyAsync(u => u.Email == email);
            if (emailExists)
                return Conflict(new MessageResponse($"Já existe uma conta cadastrada para este e-mail: {email}"));

            User user = new User(email, passwordHash);

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return Ok(new MessageResponse($"Usuário cadastrado: {email}"));
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody]LoginRequest request)
        {
            await HttpContext.SignOutAsync("Cookies");

            User? user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user is null)
                return NotFound(new MessageResponse($"Conta não cadastrada: {request.Email}"));

            if (user.IsSoftDeleted)
                return NotFound(new MessageResponse($"A conta do usuário foi excluída, verifique o motivo com a administração do sistema"));

            bool passwordVerified = BCryptNet.Verify(request.Password, user.Password);
            if (!passwordVerified)
                return Unauthorized(new MessageResponse("Senha incorreta"));

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookies", principal);

            return Ok(new MessageResponse("Sessão iniciada"));
        }

        [HttpPost("logout")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return Ok(new MessageResponse("Sessão finalizada"));
        }

        [Authorize]
        [HttpPost("change-password")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, [FromServices] IAuthenticatedUser authenticatedUser)
        {
            // TODO : neste projeto demonstrativo, a senha não é comparada com as anteiores.
            // A lógica de senha pode ser alterada para adequar um sistema real.
            int? userId = authenticatedUser.GetUserId();
            if (userId is null)
                return Unauthorized("Sessão expirada ou usuário não entrou");

            var user = await context.Users
                .Where(u => u.Id == userId)
                .Select(u => new { u.Id, u.Password })
                .FirstOrDefaultAsync();

            if (user is null)
                return NotFound("O registro do usuário não pôde ser localizado e pode ter sido excluído recentemente");

            bool passwordVerified = BCryptNet.Verify(request.OldPassword, user.Password);
            if (!passwordVerified)
                return Unauthorized("Senha incorreta");

            string newPassword = BCryptNet.HashPassword(request.NewPassword);
            EntityEntry<User> userEntity = context.Users.Attach(new User(user.Id));
            userEntity.SetProperty(u => u.Password, newPassword);

            await context.SaveChangesAsync();
            return Ok(new MessageResponse("Senha alterada"));
        }
    }
}
