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

                string[] roles = new[] { "Admin" };

                foreach (string role in roles)
                {
                    var exists = await roleManager.RoleExistsAsync(role);
                    if (exists) continue;
                    await roleManager.CreateAsync(new IdentityRole(role));
                }

                var user = new AppUser
                {
                    FirstName = "Fidan",
                    LastName = "Ismayilova",
                    Email = "fidan_ism@mail.ru",
                    UserName = "fidan_ism@mail.ru",
                };

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var existingUser = await userManager.FindByNameAsync("fidan_ism@mail.ru");
                if (existingUser is not null) return;

                await userManager.CreateAsync(user, "adeliya2000");
                await userManager.AddToRoleAsync(user, roles[0]);

                return;
            }
        }
    }
}
