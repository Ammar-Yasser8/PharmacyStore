using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Repository.SeedingData
{
    public static class IdentitySeed
    {
        public static async Task SeedUserAsync(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Ensure roles exist
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("Customer"))
                await roleManager.CreateAsync(new IdentityRole("Customer"));

            // Check if the admin user exists
            var adminUser = await userManager.FindByEmailAsync("ammar.yasser@gmail.com");
            if (adminUser == null)
            {
                var user = new AppUser
                {
                    DisplayName = "Ammar Yasser",
                    Email = "ammar.yasser@gmail.com",
                    UserName = "ammar.yasser",
                    PhoneNumber = "01127080264",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Pa$$w0rd");
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(",", result.Errors.Select(e => e.Description)));
                }

                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}