using System.ComponentModel.DataAnnotations;

namespace BellosoftWebApi.Requests
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "A senha atual não foi informada.")]
        [Length(6, 50, ErrorMessage = "A senha atual não confere.")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "A nova senha não foi informada.")]
        [Length(6, 50, ErrorMessage = "A nova senha deve conter pelo menos 6 caracteres (e não deve exceder 50 caracteres).")]
        public string NewPassword { get; set; }

        public ChangePasswordRequest(string oldPassword, string newPassword)
        {
            OldPassword = oldPassword;
            NewPassword = newPassword;
        }
    }
}
