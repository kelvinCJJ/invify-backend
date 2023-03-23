using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Domain.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public DateTime? DateTimeUpdated { get; set; }
        public DateTime? DateTimeDeleted { get; set; }

    }
}
