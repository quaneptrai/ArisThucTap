using ArisSkyve.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;

var builder = WebApplication.CreateBuilder(args);
var google = builder.Configuration.GetSection("Authentication:Google");
var facebook = builder.Configuration.GetSection("Authentication:Facebook");
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContextPool<ArisDBContext>(options =>
{
    var connection = builder.Configuration.GetConnectionString("DefaultConnection");

    Console.WriteLine(connection);

    options.UseSqlServer(connection);
});
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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}")
    .WithStaticAssets();

app.Run();