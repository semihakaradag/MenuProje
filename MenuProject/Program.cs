using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using MenuProject.Data;

var builder = WebApplication.CreateBuilder(args);

// Veritabaný baðlantýsýný ekleyelim
builder.Services.AddDbContext<MenuDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
});

// Identity servisini ekleyelim
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // E-posta doðrulamasý kapalý
})
.AddEntityFrameworkStores<MenuDbContext>()
.AddDefaultTokenProviders();

// Yetkilendirme Politikalarýný Ekleyelim
builder.Services.AddAuthorization(options =>
{
   
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireTeacherRole", policy => policy.RequireRole("Teacher"));
    options.AddPolicy("RequireStudentRole", policy => policy.RequireRole("Student"));
});

// Cookie ayarlarýný yapýlandýralým
builder.Services.ConfigureApplicationCookie(opt =>
{
    var cookieBuilder = new CookieBuilder();
    cookieBuilder.Name = "menuprojectcookie";
    opt.LoginPath = new PathString("/Account/SignIn");
    opt.LogoutPath = new PathString("/Account/Logout");
    opt.Cookie = cookieBuilder;
    opt.ExpireTimeSpan = TimeSpan.FromDays(60);
    opt.SlidingExpiration = true;
    opt.AccessDeniedPath = new PathString("/Account/AccessDenied"); // Yetkisiz eriþimler için yönlendirme
});

// Dosya saðlayýcý ekleyelim
builder.Services.AddSingleton<IFileProvider>(
    new PhysicalFileProvider(Directory.GetCurrentDirectory()));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Varsayýlan yönlendirme
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
