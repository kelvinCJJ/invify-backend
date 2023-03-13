using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invify.Domain.Entities.User.Role;
using Invify.Domain.Entities.User;
using System.Reflection.Emit;
using Invify.Domain.Entities;

namespace Invify.Infrastructure.Configuration
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<ApplicationRole>().ToTable("Roles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            builder.Entity<Category>().ToTable("Category");
            builder.Entity<Inventory>().ToTable("Inventory");
            builder.Entity<Product>().ToTable("Product");
            builder.Entity<Purchase>().ToTable("Purchase");
            builder.Entity<PurchaseDetail>().ToTable("PurchaseDetail");
            builder.Entity<Sale>().ToTable("Sale");
            builder.Entity<Supplier>().ToTable("Supplier");

        }
    }
}
    