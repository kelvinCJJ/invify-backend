using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using Invify.Domain.Entities;

namespace Invify.Infrastructure.Configuration
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Category { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Purchase> Purchase { get; set; }
        public DbSet<PurchaseDetail> PurchaseDetail { get; set; }
        public DbSet<Sale> Sale { get; set; }
        public DbSet<Supplier> Supplier { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<IdentityUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
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

        private void SeedData(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData
                (
                    new IdentityRole() {Name = "Manager", ConcurrencyStamp = "1", NormalizedName="Manager" },
                    new IdentityRole() { Name = "Admin", ConcurrencyStamp = "2", NormalizedName = "User" },
                    new IdentityRole() { Name = "Basic", ConcurrencyStamp = "3", NormalizedName = "User" }
                );

        }
    }
}
    