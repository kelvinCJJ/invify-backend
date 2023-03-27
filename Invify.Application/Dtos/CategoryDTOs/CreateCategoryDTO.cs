using Invify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Dtos.CategoryDTOs
{
    public class CreateCategoryDTO
    {
        [Required]
        [StringLength(100,ErrorMessage = "Category name cannot exceed 100 characters!")]
        public string Name { get; set; }
    }
}
