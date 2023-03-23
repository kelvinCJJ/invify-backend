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
        public DbSet<Category>? Categories { get; set; }
        public DbSet<Inventory>? Inventories { get; set; }
        public DbSet<Product>? Products { get; set; }
        public DbSet<Purchase>? Purchases { get; set; }
        public DbSet<Sale>? Sales { get; set; }
        public DbSet<Supplier>? Suppliers { get; set; }
        public DbSet<StockTake>? Stocks { get; set; }

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
            builder.Entity<Sale>().ToTable("Sale");
            builder.Entity<StockTake>().ToTable("StockTake");
            builder.Entity<Supplier>().ToTable("Supplier");

            SeedData(builder);

        }

        private void SeedData(ModelBuilder builder)
        {
            IdentityUser user = new IdentityUser()
            {
                Id = "3c54f767-53fa-476d-9578-4b4742c5089e",
                UserName = "Admin",
                Email = "2001427@sit.singaporetech.edu.sg",
                LockoutEnabled = false,
                EmailConfirmed= false,
            };

            PasswordHasher<IdentityUser> passwordHasher = new PasswordHasher<IdentityUser>();
            passwordHasher.HashPassword(user, "P@ssw0rd");

            builder.Entity<IdentityUser>().HasData(user);

            builder.Entity<IdentityRole>().HasData(
               new IdentityRole() { Id = "e44ee8f2-b407-4a9c-827f-34df3fa045ad", Name = "Basic", ConcurrencyStamp = "1", NormalizedName = "User" },
               new IdentityRole() { Id = "b71102db-b78e-4ae1-80c0-718321b42ae3", Name = "Admin", ConcurrencyStamp = "2", NormalizedName = "Admin" }
               );

            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>() { RoleId = "b71102db-b78e-4ae1-80c0-718321b42ae3", UserId = "3c54f767-53fa-476d-9578-4b4742c5089e" }
                );

            //builder.Entity<Category>().HasData
            //    (
            //        new Category() { Id = Guid.NewGuid(), Name = "Electronics",},
            //        new Category() { Id = Guid.NewGuid(), Name = "Clothing" },
            //        new Category() { Id = Guid.NewGuid(), Name = "Mask" },
            //        new Category() { Id = Guid.NewGuid(), Name = "Lifestyle" },
            //        new Category() { Id = Guid.NewGuid(), Name = "Furniture" },
            //        new Category() { Id = Guid.NewGuid(), Name = "Other" }
            //    );

            ////data seed for product
            //builder.Entity<Product>().HasData
            //    (
            //    // create product data
            //    new Product() { Id = Guid.NewGuid(), Name = "iPhone 14 Pro Max", Description = "iPhone 14 Pro Max 128GB", CategoryId = 1, Price = 1999.99M,Cost = 1799.99M, DateTimeCreated = DateTime.UtcNow.AddHours(8),},
            //    new Product() { Id = Guid.NewGuid(), Name = "iPhone 14 Pro", Description = "iPhone 12 Pro 128GB", CategoryId = 1, Price = 1799.99M, DateTimeCreated = DateTime.UtcNow.AddHours(8), },
            //    new Product() { Id = Guid.NewGuid(), Name = "iPhone 14 Plus", Description = "iPhone 12 128GB", CategoryId = 1, Price = 1599.99M, DateTimeCreated = DateTime.UtcNow.AddHours(8), },
            //    new Product() { Id = Guid.NewGuid(), Name = "iPhone 13 Pro Max", Description = "iPhone 11 Pro Max 128GB", CategoryId = 1, Price = 1799.99M, DateTimeCreated = DateTime.UtcNow.AddHours(8), },
            //    new Product() { Id = Guid.NewGuid(), Name = "iPhone 13 Pro", Description = "iPhone 11 Pro 128GB", CategoryId = 1, Price = 1599.99M, DateTimeCreated = DateTime.UtcNow.AddHours(8), },
            //    new Product() { Id = Guid.NewGuid(), Name = "iPhone 13", Description = "iPhone 11 128GB", CategoryId = 1, Price = 1399.99M, DateTimeCreated = DateTime.UtcNow.AddHours(8), }
            //    );


        }
    }
}
