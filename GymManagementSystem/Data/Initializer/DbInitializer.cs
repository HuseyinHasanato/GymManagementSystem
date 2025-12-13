using Microsoft.AspNetCore.Identity;
using GymManagementSystem.Data;
using System.Threading.Tasks;

namespace GymManagementSystem.Data.Initializer
{
    public class DbInitializer
    {
       
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        
        public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Initialize()
        {
            
            if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
                await _roleManager.CreateAsync(new IdentityRole("Uye")); // العضو (Üye)
            }

            
            var adminEmail = "test.user.sakarya@gym.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var user = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };

                var result = await _userManager.CreateAsync(user, "StrongPass123!");

                if (result.Succeeded)
                {
                    
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}