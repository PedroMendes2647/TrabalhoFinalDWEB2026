using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.WebApp.Data;
using TrabalhoFinalDWEB2026.WebApp.Hubs;
using TrabalhoFinalDWEB2026.WebApp.Models;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao contentor.
builder.Services.AddRazorPages();

// 1. Registar o serviço do SignalR
builder.Services.AddSignalR();

// Configurar a base de dados com Azure Identity (Managed Identity em Azure)
builder.Services.AddDbContext<ApplicationDbContext>(options => {
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrEmpty(connectionString) && !builder.Environment.IsDevelopment()) {
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");
    }

    options.UseSqlServer(connectionString 
        ?? "Server=(localdb)\\mssqllocaldb;Database=TrabalhoFinalDWEB2026;Trusted_Connection=True;MultipleActiveResultSets=true",
        sqlOptions => {
            sqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
        });
});

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

// Aplicar migrations da base de dados (Azure/Production)
if (!app.Environment.IsDevelopment()) {
    try {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Iniciando aplicação de migrations da base de dados...");

        using (var scope = app.Services.CreateScope()) {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            logger.LogInformation("Verificando conexão com base de dados...");
            db.Database.OpenConnection();
            db.Database.CloseConnection();
            logger.LogInformation("Conexão com base de dados OK");

            logger.LogInformation("Aplicando migrations...");
            db.Database.Migrate();
            logger.LogInformation("Migrations aplicadas com sucesso");
        }
    }
    catch (Exception ex) {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro crítico ao aplicar migrations da base de dados. Aplicação iniciará sem migrations.");
        // Não lançar exceção - permitir que a app inicie para debug
    }
}

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

// Mapear o endpoint do Hub do SignalR para comunicação em tempo real
app.MapHub<ReceitaHub>("/receitaHub");

app.MapRazorPages()
   .WithStaticAssets();

app.Run();