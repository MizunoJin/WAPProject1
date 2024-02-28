using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WADProject1.Models;

namespace WADProject1.Seeds
{
    public class SeedDatabase
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {

            using (var context = new TenderContext(
            serviceProvider.GetRequiredService<DbContextOptions<TenderContext>>()))
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string[] roleNames = { "Admin", "User" };
                foreach (var roleName in roleNames)
                {
                    var roleExist = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                if (!context.Users.Any())
                {
                    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

                    var user = new User
                    {
                        Email = "user@example.com",
                        UserName = "user@example.com",
                    };
                    userManager.CreateAsync(user, "Password123!").Wait();

                    context.UserProfiles.Add(new UserProfile
                    {
                        Name = "Sample User",
                        Detail = "Sample Details",
                        SexualOrientation = "Sample Orientation",
                        UserId = user.Id,
                        User = user
                    });

                    context.SaveChanges();
                }

                // その他のSeedデータ...
            }

        }
    }
}
