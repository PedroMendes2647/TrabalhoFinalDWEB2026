using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TrabalhoFinalDWEB2026.Data;
using TrabalhoFinalDWEB2026.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=(localdb)\\mssqllocaldb;Database=TrabalhoFinalDWEB2026;Trusted_Connection=True;MultipleActiveResultSets=true"));

builder.Services.AddDefaultIdentity<Utente>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole<string>>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// Initialize roles and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<string>>>();

    // Create roles if they don't exist
    string[] roles = { "Utente", "Doutor", "Farmaceuta" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<string> { Id = Guid.NewGuid().ToString(), Name = role });
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

app.Run();
