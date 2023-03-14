

using System.ComponentModel.DataAnnotations;

namespace invify_backend.Dtos.Authentication
{
    public class RegisterUserDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


    }
}
