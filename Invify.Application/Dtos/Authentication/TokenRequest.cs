using System.ComponentModel.DataAnnotations;

namespace Invify.Dtos.Authentication
{
    public class TokenRequest
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
