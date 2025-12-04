using Microsoft.AspNetCore.Identity;

namespace GymManagementSystem.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            // استدعاء مديري المستخدمين والأدوار
            var userManager = service.GetService<UserManager<IdentityUser>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();

            // 1. إنشاء الأدوار (Roles) إذا لم تكن موجودة
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("Member"))
                await roleManager.CreateAsync(new IdentityRole("Member"));

            // 2. إنشاء مستخدم الأدمن (Admin)
            // 🛑 هام: استبدل النص أدناه برقمك الجامعي الحقيقي
            var adminEmail = "huseyin.hasanato@ogr.sakarya.edu.tr";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var user = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                // إنشاء المستخدم بكلمة مرور "sau"
                var result = await userManager.CreateAsync(user, "sau");

                if (result.Succeeded)
                {
                    // تعيين رتبة الأدمن له
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}