using System.ComponentModel.DataAnnotations;

namespace invify_backend.Dtos.Authentication
{
    public class LoginRequestDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
