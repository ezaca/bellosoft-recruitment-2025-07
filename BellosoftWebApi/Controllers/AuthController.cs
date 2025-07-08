using BellosoftWebApi.Facades;
using BellosoftWebApi.Models;
using BellosoftWebApi.Requests;
using BellosoftWebApi.Responses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BCryptNet = BCrypt.Net.BCrypt;

namespace BellosoftWebApi.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
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

        [HttpPost("change-password")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, [FromServices] IAuthenticatedUser authenticatedUser)
        {
            // TODO : neste projeto demonstrativo, a senha não é comparada com as anteiores.
            // A lógica de senha pode ser alterada para adequar um sistema real.
            User? user = await authenticatedUser.GetActiveUser();

            if (user is null)
                return Unauthorized("Sessão expirada ou usuário não entrou");
            
            bool passwordVerified = BCryptNet.Verify(request.OldPassword, user.Password);
            if (!passwordVerified)
                return Unauthorized("Senha incorreta");

            user.Password = BCryptNet.HashPassword(request.NewPassword);
            await context.SaveChangesAsync();
            return Ok(new MessageResponse("Senha alterada"));
        }
    }
}
