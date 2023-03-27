using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Dtos.Authentication
{
    public class UpdateUserDTO
    {
        [Required]
        public string Email { get; set; }
        
        [Required]
        [StringLength(50, ErrorMessage = "Username must be less then 50 characters")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]        
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

    }


}
