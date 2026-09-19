using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.WebApp.Data;
using TrabalhoFinalDWEB2026.WebApp.Hubs;
using TrabalhoFinalDWEB2026.WebApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao contentor.
builder.Services.AddRazorPages();

// 1. Registar o serviço do SignalR
builder.Services.AddSignalR();

// Configurar a base de dados
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Server=(localdb)\\mssqllocaldb;Database=TrabalhoFinalDWEB2026;Trusted_Connection=True;MultipleActiveResultSets=true"));

// Configurar identidade
builder.Services.AddIdentity<Utente, IdentityRole<string>>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configurar cookies de autenticação
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Configurar o pipeline de requisições HTTP.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// 2. Mapear o endpoint do Hub do SignalR para comunicação em tempo real
app.MapHub<ReceitaHub>("/receitaHub");

app.MapRazorPages()
   .WithStaticAssets();

app.Run();