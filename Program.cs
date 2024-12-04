using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Proiect_MPA.Data;
using Proiect_MPA.Hubs;
using Microsoft.AspNetCore.Identity;
using Proiect_MPA.Models;
using Microsoft.Extensions.Options;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<RestaurantContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RestaurantContext") ?? throw new InvalidOperationException("Connection string 'RestaurantContext' not found.")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
       .AddRoles<IdentityRole>()
       .AddEntityFrameworkStores<IdentityContext>();
builder.Services.AddDbContext<IdentityContext>(options =>

options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddDefaultIdentity<IdentityUser>(options
//=> options.SignIn.RequireConfirmedAccount = true);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddRazorPages();

builder.Services.AddAuthorization(options =>
{
    // Politica pentru Manager: Acces complet
    options.AddPolicy("ManagerAccess", policy =>
        policy.RequireRole("Manager"));

    // Politica pentru Angajat: Acces doar la citire
    options.AddPolicy("EmployeeAccess", policy =>
        policy.RequireRole("Employee"));

    // Politia? pentru Client: Poate vedea doar propriile rezervari
    options.AddPolicy("ClientAccess", policy =>
        policy.RequireRole("Client"));

    
    
});

builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.AccessDeniedPath = "/Identity/Account/AccessDenied";

});
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    DbInitializer.Initialize(services);
}

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

app.MapHub<ChatHub>("/Chat");
app.MapRazorPages();
app.Run();
