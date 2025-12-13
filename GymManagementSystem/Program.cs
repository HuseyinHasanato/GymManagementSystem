using GymManagementSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Data.Initializer; // 1. ? «” Ì—«œ ›∆… Initializer

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 2. ? ≈÷«›… œ⁄„ «·√œÊ«— (Roles) Ê ”ÃÌ· Œœ„… Initializer
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // ? Â–« ÷—Ê—Ì ·«” Œœ«„ RoleManager
    .AddEntityFrameworkStores<ApplicationDbContext>();

//  ”ÃÌ· Œœ„… DbInitializer ··«” Œœ«„ ⁄»— Dependency Injection
builder.Services.AddScoped<DbInitializer>(); // ?  ”ÃÌ· «·Œœ„…

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ----------------------------------------------------------------------
// 3. ? «” œ⁄«¡ œ«·…  ÂÌ∆… «·√œÊ«— Êﬁ«⁄œ… «·»Ì«‰«  ⁄‰œ »œ¡ «· ‘€Ì·
// ----------------------------------------------------------------------
await InitializeDatabase(app);
// ----------------------------------------------------------------------


// Configure the HTTP request pipeline.
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

// 4. ? «· — Ì» «·’ÕÌÕ ··√„«‰: UseAuthentication ÌÃ» √‰ ÌﬂÊ‰ ﬁ»· UseAuthorization
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();


// ----------------------------------------------------------------------
// 5. ? «·œ«·… «·„”«⁄œ… · ÂÌ∆… ﬁ«⁄œ… «·»Ì«‰«  (Roles & Admin User)
// ----------------------------------------------------------------------
async Task InitializeDatabase(IHost host)
{
    using (var scope = host.Services.CreateScope())
    {
        var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
        await initializer.Initialize(); // «” œ⁄«¡ œ«·… Initialize
    }
}