using System;
using System.Collections.Generic;
using ImageSharingWithSecurity.DAL;
using ImageSharingWithSecurity.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure cookie policy to allow ADA saved in a cookie
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

// Configure logging to go to the console (local testing only!)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Connection string for SQL database
string dbConnectionString = builder.Configuration["Data:ApplicationDb:ConnectionString"];
if (dbConnectionString == null) throw new KeyNotFoundException("Missing database connection string");
var connStringBuilder = new SqlConnectionStringBuilder(dbConnectionString);

string database = builder.Configuration["Data:ApplicationDb:Database"];
if (database == null) throw new KeyNotFoundException("Missing database name");
connStringBuilder.InitialCatalog = database;

string dbUser = builder.Configuration["Credentials:ApplicationDb:User"];
if (dbUser == null) throw new KeyNotFoundException("Missing database username");
connStringBuilder.UserID = dbUser;

string dbPassword = builder.Configuration["Credentials:ApplicationDb:Password"];
if (dbPassword == null) throw new KeyNotFoundException("Missing database password");
connStringBuilder.Password = dbPassword;

dbConnectionString = connStringBuilder.ConnectionString;

// Add database context & enable saving data in the log (for development only)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(dbConnectionString);
    options.EnableSensitiveDataLogging();
});

// Add Identity service
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => 
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 4;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    }).AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseCookiePolicy();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<ApplicationDbInitializer>>();
    var db = services.GetRequiredService<ApplicationDbContext>();
    var initializer = new ApplicationDbInitializer(db, logger);

    await initializer.SeedDatabase(services);  // Only call this once
}


app.Run();
