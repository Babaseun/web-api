using ApiProject.DataAccess;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProject.Models
{
    public static class PreSeeder
    {
        public static async Task Seeder(AppDbContext ctx, RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            ctx.Database.EnsureCreated();

            if (!roleManager.Roles.Any())
            {
                var listOfRoles = new List<IdentityRole>
                {
                    new IdentityRole("Admin"),
                    new IdentityRole("Customer")
                };

                foreach (var role in listOfRoles) await roleManager.CreateAsync(role);
            }

            if (!userManager.Users.Any())
            {
                var listOfUsers = new List<ApplicationUser>
                {
                    new ApplicationUser
                    {
                        UserName = "James", Email = "james@me.com", LastName = "Bond", FirstName = "James",
                        Photo = "~/images/avarta.jpg"
                    },
                    new ApplicationUser
                    {
                        UserName = "blade@me.com", Email = "blade@me.com", LastName = "Snipes", FirstName = "Wesly",
                        Photo = "~/images/avarta.jpg"
                    }
                };

                var counter = 0;
                foreach (var user in listOfUsers)
                {
                    var result = await userManager.CreateAsync(user, "P@$$word1");

                    if (result.Succeeded)
                    {
                        if (counter == 0)
                            await userManager.AddToRoleAsync(user, "Admin");
                        else
                            await userManager.AddToRoleAsync(user, "Customer");
                    }

                    counter++;
                }
            }
        }
    }
}