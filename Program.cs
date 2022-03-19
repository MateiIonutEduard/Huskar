using Huskar.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Huskar.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MovieContext>(options =>
    options.UseMySql(config.GetConnectionString("MovieDB"),
            new MySqlServerVersion(new Version(10, 4, 22))
            ));

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
           .AddGoogle(options =>
           {
               IConfigurationSection googleAuthNSection =
               config.GetSection("Google");
               options.ClientId = googleAuthNSection["ClientId"];
               options.ClientSecret = googleAuthNSection["ClientSecret"];
               options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
           })
           .AddCookie(options =>
           {
               options.LoginPath = "/account/facebook-login";
           })
           .AddFacebook(options =>
           {
               IConfigurationSection FBAuthNSection =
               config.GetSection("Facebook");
               options.ClientId = FBAuthNSection["AppId"];
               options.ClientSecret = FBAuthNSection["AppSecret"];
               options.Scope.Add("public_profile");
               options.Fields.Add("picture");
           });

builder.Services.AddSingleton<MovieService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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