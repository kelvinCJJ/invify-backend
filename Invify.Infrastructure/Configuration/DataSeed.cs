using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Infrastructure.Configuration
{
    public class DataSeed
    {
        /// <summary>
        ///     Seed users and roles in the Identity database.
        /// </summary>
        /// <param name="userManager">ASP.NET Core Identity User Manager</param>
        /// <param name="roleManager">ASP.NET Core Identity Role Manager</param>
        /// <returns></returns>
        public static async Task SeedAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Add roles supported
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            await roleManager.CreateAsync(new IdentityRole("Basic"));

            // New admin user
            string adminUserName = "admin";
            var adminUser = new IdentityUser
            {
                UserName = adminUserName,
                Email = "2001427@sit.singaporetech.edu.sg",
                EmailConfirmed = true,
                LockoutEnabled= false,
                
            };

            // Add new user and their role
            await userManager.CreateAsync(adminUser, "P@ssw0rd");
            adminUser = await userManager.FindByNameAsync(adminUserName);
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

}
