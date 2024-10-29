using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_kmodi.Data;
//using Fall2024_Assignment3_kmodi.Services; // Import the namespace for AIReviewService
using Fall2024_Assignment3_kmodi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole().SetMinimumLevel(LogLevel.Debug);

// Retrieve the base connection string without credentials from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Retrieve sensitive information (User ID, Password) from Secret Manager
var userId = builder.Configuration["ConnectionStrings:UserId"];
var password = builder.Configuration["ConnectionStrings:Password"];

// Dynamically build the full connection string
var fullConnectionString = $"{connectionString};User Id={userId};Password={password};";

///////////////////
builder.Configuration.AddJsonFile("appsettings.json");
//////////////////

// Add services to the container with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(fullConnectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();



//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Fall2024_Assignment3_kmodi.Data;
////using Fall2024_Assignment3_kmodi.Services; // Import the namespace for AIReviewService

//var builder = WebApplication.CreateBuilder(args);

//// Retrieve the base connection string without credentials from appsettings.json
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
//                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//// Retrieve sensitive information (User ID, Password) from Secret Manager
//var userId = builder.Configuration["ConnectionStrings:UserId"];
//var password = builder.Configuration["ConnectionStrings:Password"];

//// Dynamically build the full connection string
//var fullConnectionString = $"{connectionString};User Id={userId};Password={password};";

//// Add services to the container with SQL Server
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(fullConnectionString));

//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

//builder.Services.AddControllersWithViews();

//// Register AIReviewService for dependency injection
//builder.Services.AddSingleton<AIReviewService>();

//// Add Azure OpenAI settings from appsettings.json to IConfiguration
//builder.Services.Configure<OpenAISettings>(builder.Configuration.GetSection("AzureOpenAI"));

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseMigrationsEndPoint();
//}
//else
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
//app.MapRazorPages();

//app.Run();
