using System.ComponentModel.DataAnnotations;

namespace BellosoftWebApi.Requests
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "O e-mail não foi informato.")]
        [EmailAddress(ErrorMessage = "O e-mail precisa ter um formato válido: exemplo@exemplo.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha não foi informada.")]
        [Length(6, 50, ErrorMessage = "A senha deve conter pelo menos 6 caracteres (e não deve exceder 50 caracteres).")]
        public string Password { get; set; }

        public LoginRequest(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
