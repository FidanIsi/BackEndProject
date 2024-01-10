using Microsoft.AspNetCore.Identity;
using WebApplication1.Entities;

namespace WebApplication1.Helper
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                string roleName = "Admin";

                var roleExists = await roleManager.RoleExistsAsync(roleName);

                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }

                var user = new AppUser
                {
                    FirstName = "Fidan",
                    LastName = "Ismayilova",
                    Email = "fidan_ism@mail.ru",
                    UserName = "fidan_ism@mail.ru",
                };

                var existingUser = await userManager.FindByNameAsync("fidan_ism@mail.ru");
                if (existingUser == null)
                {
                    var result = await userManager.CreateAsync(user, "adeliya2000");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, roleName);
                    }
                    else
                    {
                        throw new ApplicationException($"Error creating user: {string.Join(", ", result.Errors)}");
                    }
                }
            }
        }
    }
}
