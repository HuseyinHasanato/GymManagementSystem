using Microsoft.AspNetCore.Identity;
using GymManagementSystem.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GymManagementSystem.Data.Initializer
{
    public class DbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db; // إضافة السياق للترحيلات

        public DbInitializer(
            ApplicationDbContext db,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Initialize()
        {
            // 1. تطبيق الترحيلات (Migrations) المعلقة
            try
            {
                if (_db.Database.GetPendingMigrations().Any())
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception)
            {
                // يجب معالجة أخطاء الترحيل هنا إذا لزم الأمر
            }

            // 2. التحقق من وجود الأدوار (نستخدم "Admin" كنقطة تحقق)
            if (await _roleManager.RoleExistsAsync("Admin"))
            {
                // إذا كان دور Admin موجوداً، فهذا يعني أن الأدوار الأخرى قد أنشئت مسبقاً.
                return;
            }

            // 3. إنشاء الأدوار المطلوبة (يتم توحيد اسم العضو إلى "Member")
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
            await _roleManager.CreateAsync(new IdentityRole("Member")); // تم توحيد الاسم ليتوافق مع "RegisterModel.cs"

            // ملاحظة: تم إزالة منطق إنشاء المستخدم "test.user.sakarya@gym.com" من هنا.
            // هذا المنطق تم نقله إلى Program.cs لتنفيذ طلب التحديث الخاص بك،
            // مما يمنع التكرار والتعارض.
        }
    }
}