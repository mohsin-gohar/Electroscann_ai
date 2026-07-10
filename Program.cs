using ElectroScanAI.Models.Entities;
using Electroscann_ai.Data;
using Electroscann_ai.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ========== ADD SERVICES ==========

// Add Controllers with Views
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<ElectroscannDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbcs")));

// Add Authentication with Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// Add Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ElectricianOnly", policy => policy.RequireRole("Electrician"));
    options.AddPolicy("CompanyOnly", policy => policy.RequireRole("Company"));
    options.AddPolicy("ClientOnly", policy => policy.RequireRole("Client"));
});

// Add Password Hasher for custom user authentication
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Add Session support (optional, for temp data)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add HttpContextAccessor for accessing current user
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ========== CONFIGURE PIPELINE ==========

// Configure error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Security middleware
app.UseHttpsRedirection();
app.UseStaticFiles();

// Routing middleware
app.UseRouting();

// Authentication & Authorization (ORDER MATTERS!)
app.UseAuthentication();  // First Authentication
app.UseAuthorization();   // Then Authorization
app.UseSession();         // Then Session

// ========== MAP ROUTES ==========

// Dashboard routes
app.MapControllerRoute(
    name: "Dashboard",
    pattern: "Dashboard/{action=Index}/{id?}",
    defaults: new { controller = "Dashboard" });

// Account routes (Login, Register)
app.MapControllerRoute(
    name: "account",
    pattern: "Account/{action=Login}/{id?}",
    defaults: new { controller = "Account" });

// Electrician Dashboard routes
app.MapControllerRoute(
    name: "electrician",
    pattern: "Electrician/{action=Index}/{id?}",
    defaults: new { controller = "ElectricianDashboard" });

// Company Dashboard routes
app.MapControllerRoute(
    name: "company",
    pattern: "Company/{action=Index}/{id?}",
    defaults: new { controller = "CompanyDashboard" });

// Default route (Home page)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();