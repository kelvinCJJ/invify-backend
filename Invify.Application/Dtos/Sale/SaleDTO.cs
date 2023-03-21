using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Dtos.Sale
{
    public class SaleDTO
    {
        [Required]
        public List<int> ProductIds { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public DateTime SaleDate { get; set; }
    }
}
