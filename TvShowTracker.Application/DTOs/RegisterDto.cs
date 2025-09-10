using System.ComponentModel.DataAnnotations;

namespace TvShowTracker.Application.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username é obrigatório")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username deve ter entre 3 e 50 caracteres")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username só pode conter letras, números e underscore")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(256, ErrorMessage = "Email não pode exceder 256 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password deve ter pelo menos 6 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
            ErrorMessage = "Password deve conter pelo menos: 1 letra minúscula, 1 maiúscula, 1 número e 1 caractere especial")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmação de password é obrigatória")]
        [Compare("Password", ErrorMessage = "Password e confirmação não coincidem")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
