using GymManagementSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ≈⁄œ«œ «·« ’«· »ﬁ«⁄œ… «·»Ì«‰« 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- ≈⁄œ«œ«  «·ÂÊÌ… ( Œ›Ì› ‘—Êÿ «·»«”Ê—œ) ---
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false; // ·« Ì ÿ·»  √ﬂÌœ ≈Ì„Ì·
    options.Password.RequireDigit = false;          // ·« Ì ÿ·» √—ﬁ«„
    options.Password.RequireLowercase = false;      // ·« Ì ÿ·» Õ—Ê› ’€Ì—…
    options.Password.RequireNonAlphanumeric = false;// ·« Ì ÿ·» —„Ê“ (!@#)
    options.Password.RequireUppercase = false;      // ·« Ì ÿ·» Õ—Ê› ﬂ»Ì—…
    options.Password.RequiredLength = 3;            // «·ÿÊ· «·„”„ÊÕ 3 Õ—Ê› (·√Ã· sau)
})
.AddRoles<IdentityRole>() //  ›⁄Ì· ‰Ÿ«„ «·√œÊ«—
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// --- ﬂÊœ  ‘€Ì· «·‹ Seeder ·≈‰‘«¡ «·√œ„‰ ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbSeeder.SeedRolesAndAdminAsync(services);
}
// ------------------------------------------

// ≈⁄œ«œ«  «·‹ Pipeline
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();