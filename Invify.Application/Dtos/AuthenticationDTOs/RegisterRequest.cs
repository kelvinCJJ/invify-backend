using System.ComponentModel.DataAnnotations;

namespace Invify.Dtos.Authentication
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Length must be less then 50 characters")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
