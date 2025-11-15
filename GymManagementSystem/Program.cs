using GymManagementSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Data.Initializer;
using GymManagementSystem.Services; // IAIService için servisler

var builder = WebApplication.CreateBuilder(args);

// Hizmetleri kapsayıcıya ekle.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 2. Kimlik (Identity) Servislerini Yapılandırma
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // ÖNEMLİ: Yönetici şifresi 'SAU' için gerekli ayarlar
    options.SignIn.RequireConfirmedAccount = false; // E-posta onayı gerekliliğini kapat (Önceki istek üzerine)

    // Şifre Gereksinimleri (Kullanıcı isteği olan 'SAU' şifresi için ayarlandı)
    options.Password.RequireLowercase = false;          // Küçük harf zorunluluğunu kapat
    options.Password.RequireUppercase = true;           // Büyük harf zorunluluğunu açık tut (SAU içeriyor)
    options.Password.RequireDigit = false;              // Rakam zorunluluğunu kapat
    options.Password.RequiredLength = 3;                // En az 3 karakter uzunluğu ayarla (SAU için)
    options.Password.RequireNonAlphanumeric = false;    // Sembol zorunluluğunu kapat
})
.AddRoles<IdentityRole>() // RoleManager servisini ekler (Rolleri yönetmek için)
.AddEntityFrameworkStores<ApplicationDbContext>();

// ********** Yapay Zeka (AI) Entegrasyonu **********

// 3. AIService'in IHttpClientFactory üzerinden HTTP istekleri yapabilmesi için HttpClient servisini ekle
builder.Services.AddHttpClient();

// 4. Yapay Zeka Servisini kaydet (IAIService arayüzünü AIService sınıfına bağlar)
builder.Services.AddScoped<IAIService, AIService>();

// **************************************************

// DbInitializer servisini (Seed Data) Dependency Injection'a ekle
builder.Services.AddScoped<DbInitializer>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Veritabanı başlatma ve Roller/Kullanıcılar oluşturma işlemini çağır
await InitializeDatabase(app);


if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();


// *******************************************************************
// VERİTABANI BAŞLATMA VE ROL/ADMİN KULLANICI OLUŞTURMA MANTIĞI
// *******************************************************************
async Task InitializeDatabase(IHost host)
{
    using (var scope = host.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var initializer = services.GetRequiredService<DbInitializer>();

        // 1. DbInitializer'ı Çalıştır (Gerekli tüm rolleri ve initial data'yı oluşturur)
        await initializer.Initialize();

        // 2. YÖNETİCİ HESABINI GÜNCELLE/OLUŞTUR (Önceki istek üzerine)
        var adminEmail = "huseyin.hasanato@ogr.sakarya.edu.tr";
        var adminPassword = "SAU"; // Kullanıcının talep ettiği şifre

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            // Kullanıcı yoksa oluştur
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine($"Admin hesabı oluşturuldu ve rol atandı: {adminEmail}");
            }
        }
        else
        {
            // Kullanıcı varsa bilgileri güncelle (Özellikle şifreyi güncelle)
            await userManager.SetEmailAsync(adminUser, adminEmail);
            await userManager.SetUserNameAsync(adminUser, adminEmail);

            var token = await userManager.GeneratePasswordResetTokenAsync(adminUser);
            var resetResult = await userManager.ResetPasswordAsync(adminUser, token, adminPassword);

            if (resetResult.Succeeded)
            {
                Console.WriteLine($"Admin hesabının bilgileri başarıyla güncellendi: {adminEmail}");
            }

            // Rolün atanmış olduğundan emin ol
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}