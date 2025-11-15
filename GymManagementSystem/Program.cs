using GymManagementSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Data.Initializer;
using GymManagementSystem.Services; // «” Ì—«œ «·Œœ„«  (·‹ IAIService)
//  „ Õ–›: using OpenAI.Interfaces; 
//  „ Õ–›: using OpenAI; 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 2. ≈⁄œ«œ«  «·ÂÊÌ… (Identity) «·‰Â«∆Ì…
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;

})
.AddRoles<IdentityRole>() // ÷—Ê—Ì ·«” Œœ«„ RoleManager Êœ⁄„ «·√œÊ«—
.AddEntityFrameworkStores<ApplicationDbContext>();

// ********** ≈÷«›… Œœ„«  «·–ﬂ«¡ «·«’ÿ‰«⁄Ì (AI Integration) **********

// 3. ‰ﬁÊ„ ›ﬁÿ »≈÷«›… Œœ„… HttpClient ·œ⁄„ «·« ’«· «·„»«‘— „‰ AIService
builder.Services.AddHttpClient();

// 4.  ”ÃÌ· Œœ„ ‰« «·„Œ’’… ··‹ AI (· „ﬂÌ‰ Õﬁ‰ «· »⁄Ì… ›Ì ÊÕœ… «· Õﬂ„)
// Â–Â «·Œœ„…  ⁄ „œ «·¬‰ ⁄·Ï HttpClient Ê IConfiguration
builder.Services.AddScoped<IAIService, AIService>();

// ********************************************************************

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