using ArisSkyve.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CẤU HÌNH SERVICES ---
builder.Services.AddControllersWithViews();

// Lấy thông tin Auth từ appsettings.json
var google = builder.Configuration.GetSection("Authentication:Google");
var facebook = builder.Configuration.GetSection("Authentication:Facebook");

// DATABASE: Cấu hình tự động thử lại khi mạng lag (Retry)
builder.Services.AddDbContextPool<ArisDBContext>(options =>
{
    var connection = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connection, sqlServerOptionsAction: sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    });
});

// AUTHENTICATION: Cấu hình Cookie an toàn cho Cloud
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
    // Cookie cần thiết để chạy qua Proxy/HTTPS của Host
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
})
.AddGoogle(options =>
{
    options.ClientId = google["ClientId"]!;
    options.ClientSecret = google["ClientSecret"]!;
    options.CallbackPath = "/signin-google";
})
.AddFacebook(options =>
{
    options.AppId = facebook["ClientId"]!;
    options.AppSecret = facebook["ClientSecret"]!;
    options.CallbackPath = "/signin-facebook";
});

builder.Services.AddAuthorization();

var app = builder.Build();

// --- 2. CẤU HÌNH MIDDLEWARE (THỨ TỰ CỰC KỲ QUAN TRỌNG) ---

// Hiện lỗi chi tiết để debug (Xóa dòng này khi web đã chạy ổn định)
app.UseDeveloperExceptionPage();

// PHẢI LÀ DÒNG ĐẦU TIÊN để xử lý IP/Protocol từ Proxy của MonsterASP
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// FIX LỖI CORRELATION FAILED: Ép Scheme là https cho mọi Request trên Cloud
app.Use((context, next) =>
{
    context.Request.Scheme = "https";
    return next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

// Authentication PHẢI nằm TRƯỚC Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();