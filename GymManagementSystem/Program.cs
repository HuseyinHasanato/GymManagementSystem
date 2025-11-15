using GymManagementSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Data.Initializer; // «” Ì—«œ ›∆… Initializer

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 2. ≈⁄œ«œ«  «·ÂÊÌ… (Identity) «·‰Â«∆Ì… ·Õ· „‘«ﬂ·  ”ÃÌ· «·œŒÊ·
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // «·Õ· «·√Ê·:  ⁄ÿÌ· ‘—ÿ  √ﬂÌœ «·»—Ìœ «·≈·ﬂ —Ê‰Ì (· ›«œÌ Invalid login attempt)
    options.SignIn.RequireConfirmedAccount = false;

    // «·Õ· «·À«‰Ì:  Œ›Ì› „ ÿ·»«  ﬂ·„… «·”— (· ›«œÌ Œÿ√ NonAlphanumeric)
    options.Password.RequireNonAlphanumeric = false; // <-- Â–« ÂÊ «· ⁄œÌ· «·Õ«”„
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;

})
.AddRoles<IdentityRole>() // ÷—Ê—Ì ·«” Œœ«„ RoleManager Êœ⁄„ «·√œÊ«—
.AddEntityFrameworkStores<ApplicationDbContext>();

//  ”ÃÌ· Œœ„… DbInitializer ··«” Œœ«„ ⁄»— Dependency Injection
builder.Services.AddScoped<DbInitializer>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

//  ÂÌ∆… ﬁ«⁄œ… «·»Ì«‰«  ⁄‰œ «· ‘€Ì· (Seed Data)
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


async Task InitializeDatabase(IHost host)
{
    using (var scope = host.Services.CreateScope())
    {
        var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
        await initializer.Initialize();
    }
}