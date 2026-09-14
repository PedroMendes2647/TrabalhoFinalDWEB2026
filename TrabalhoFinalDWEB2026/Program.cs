using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.Data;
using TrabalhoFinalDWEB2026.Models;

var builder = WebApplication.CreateBuilder(args);

// ===== CONFIGURAÇÃO DE SERVIÇOS =====

// Controladores MVC com filtro global de autorização
builder.Services.AddControllersWithViews(options => {
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

// Configuração da Base de Dados
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Server=(localdb)\\mssqllocaldb;Database=TrabalhoFinalDWEB2026;Trusted_Connection=True;MultipleActiveResultSets=true"));

// Configuração nativa do ASP.NET Core Identity
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

// Configuração de Cookies de Autenticação do Identity
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = true;
});

var app = builder.Build();

// ===== SEEDING DE DADOS INICIAIS =====

using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<string>>>();
    var userManager = services.GetRequiredService<UserManager<Utente>>();
    var context = services.GetRequiredService<ApplicationDbContext>();

    // 1. Criar Roles
    string[] roles = { "Utente", "Doutor", "Farmaceuta" };
    foreach (var role in roles) {
        if (!await roleManager.RoleExistsAsync(role)) {
            await roleManager.CreateAsync(new IdentityRole<string> { Id = Guid.NewGuid().ToString(), Name = role });
        }
    }

    // 2. Criar Utilizadores Padrão (Doutores e Farmaceutas usam as respetivas subclasses)
    if (await userManager.FindByNameAsync("000000001") == null) {
        var utente = new Utente {
            Id = "000000001",
            UserName = "000000001",
            NumeroUtente = "000000001",
            Nome = "Utilizador Teste",
            Email = "000000001@exemplo.com",
            DataNascimento = new DateTime(1990, 1, 1),
            EmailConfirmed = true
        };
        var res = await userManager.CreateAsync(utente, "Senha@123");
        if (res.Succeeded) await userManager.AddToRoleAsync(utente, "Utente");
    }

    if (await userManager.FindByNameAsync("000000002") == null) {
        var doutor = new Doutor {
            Id = "000000002",
            UserName = "000000002",
            NumeroUtente = "000000002",
            Nome = "Médico Teste",
            Email = "000000002@exemplo.com",
            DataNascimento = new DateTime(1985, 5, 15),
            EmailConfirmed = true
        };
        var res = await userManager.CreateAsync(doutor, "Senha@123");
        if (res.Succeeded) {
            await userManager.AddToRoleAsync(doutor, "Utente");
            await userManager.AddToRoleAsync(doutor, "Doutor");
        }
    }

    if (await userManager.FindByNameAsync("000000003") == null) {
        var farmaceuta = new Farmaceuta {
            Id = "000000003",
            UserName = "000000003",
            NumeroUtente = "000000003",
            Nome = "Farmacêutico Teste",
            Email = "000000003@exemplo.com",
            DataNascimento = new DateTime(1988, 10, 20),
            EmailConfirmed = true
        };
        var res = await userManager.CreateAsync(farmaceuta, "Senha@123");
        if (res.Succeeded) {
            await userManager.AddToRoleAsync(farmaceuta, "Utente");
            await userManager.AddToRoleAsync(farmaceuta, "Farmaceuta");
        }
    }

    // 3. Criar Medicamentos Iniciais
    if (!context.Medicamentos.Any()) {
        var medicamentos = new[]
        {
            new Medicamentos { Nome = "Paracetamol", Tipo = "Comprimido", Dosagem = "500mg" },
            new Medicamentos { Nome = "Ibuprofeno", Tipo = "Comprimido", Dosagem = "400mg" },
            new Medicamentos { Nome = "Amoxicilina", Tipo = "Cápsula", Dosagem = "500mg" },
            new Medicamentos { Nome = "Dipirona", Tipo = "Gotas", Dosagem = "500mg/ml" },
            new Medicamentos { Nome = "Omeprazol", Tipo = "Cápsula", Dosagem = "20mg" }
        };

        foreach (var med in medicamentos) {
            context.Medicamentos.Add(med);
        }

        await context.SaveChangesAsync();
    }
}

// ===== PIPELINE HTTP =====

if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
