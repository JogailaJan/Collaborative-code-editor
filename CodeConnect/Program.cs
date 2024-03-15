using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CodeConnect.Database;
using CodeConnect.Areas.Identity.Data;
using CodeConnect.Hubs;
using CodeConnect.Infrastructure.Repository;
using CodeConnect.Infrastructure.Respository;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AuthDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AuthDbContextConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddSession();
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

builder.Services.AddTransient<IChatRepository, ChatRepository>();


//For messaging
builder.Services.AddSignalR();

builder.Services.AddLogging(config =>
{
    config.AddDebug();
    config.AddConsole();
    // Additional configuration for logging
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("https://localhost:7259") // Adjust the URL to your client's URL
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
});




var app = builder.Build();

// And enable CORS in the pipeline
app.UseCors("AllowSpecificOrigin");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapHub<ChatHub>("/chatHub");

app.Run();
