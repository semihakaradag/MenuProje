using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using MenuProject.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using MenuProject.Services;
using MenuProject.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Veritabanı bağlantısını ekleyelim
builder.Services.AddDbContextFactory<MenuDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
});

// Identity servisini ekleyelim
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // E-posta doğrulaması kapalı
})
.AddEntityFrameworkStores<MenuDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<ICourseService, CourseService>();

builder.Services.AddScoped<IncompleteUserFilterAttribute>();

builder.Services.AddScoped<AuthService>();



builder.Services.AddScoped<MissingTeacherInfoFilter>();



// Yetkilendirme Politikalarını Ekleyelim
builder.Services.AddAuthorization(options =>
{
   
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireTeacherRole", policy => policy.RequireRole("Teacher"));
    options.AddPolicy("RequireStudentRole", policy => policy.RequireRole("Student"));
});
builder.Services.AddScoped<ClaimsService>();
// Cookie ayarlarını yapılandıralım
builder.Services.ConfigureApplicationCookie(opt =>
{
    var cookieBuilder = new CookieBuilder();
    cookieBuilder.Name = "menuprojectcookie";
    opt.LoginPath = new PathString("/Account/SignIn");
    opt.LogoutPath = new PathString("/Account/Logout");
    opt.Cookie = cookieBuilder;
    opt.ExpireTimeSpan = TimeSpan.FromDays(60);
    opt.SlidingExpiration = true;
    opt.AccessDeniedPath = new PathString("/Account/AccessDenied"); // Yetkisiz erişimler için yönlendirme
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Account/SignIn"; // Kullanıcı giriş yapmadıysa yönlendirilecek sayfa
    options.LogoutPath = "/Account/Logout"; // Çıkış yaparken yönlendirilecek sayfa
    options.AccessDeniedPath = "/Account/AccessDenied"; // Yetkisiz erişim olursa yönlendirme
});


// Dosya sağlayıcı ekleyelim
builder.Services.AddSingleton<IFileProvider>(
    new PhysicalFileProvider(Directory.GetCurrentDirectory()));

builder.Services.AddControllersWithViews();




var app = builder.Build();

var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

 
    // **Admin Kullanıcısını ve Rolünü Ekleme**
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    string adminEmail = "admin@menuproject.com";
    string adminPassword = "Admin@123"; // Güçlü bir şifre belirleyebilirsin

    if (roleManager.FindByNameAsync("Admin") == null)
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    if (userManager.FindByEmailAsync(adminEmail) == null)
    {
        var adminUser = new IdentityUser
        {
            UserName = "AdminUser",
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }


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



app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");



// Varsayılan yönlendirme
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
