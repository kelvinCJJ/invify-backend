using System.ComponentModel.DataAnnotations;

namespace Invify.Dtos.Authentication
{
    public class AuthenticationRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
