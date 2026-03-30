using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebPhanTich.Models;

var builder = WebApplication.CreateBuilder(args);
// Thêm HttpClientFactory
builder.Services.AddHttpClient();


// Thêm d?ch v? MVC
builder.Services.AddControllersWithViews();

// K?t n?i c? s? d? li?u
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// C?u hình Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// C?u hình ???ng d?n ??ng nh?p/không có quy?n
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// ?? T?o c? s? d? li?u và seed d? li?u m?u khi kh?i ??ng
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();

    // T?o database n?u ch?a có
    context.Database.EnsureCreated();

    // G?i seed d? li?u m?u (ví d? “Just Ver 1.1”)
    WebPhanTich.Data.DataSeeder.Seed(context);
}

// C?u hình pipeline
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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
