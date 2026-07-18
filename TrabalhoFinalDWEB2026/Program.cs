using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.Data;
using TrabalhoFinalDWEB2026.Models;

var builder = WebApplication.CreateBuilder(args);

// ===== CONFIGURAÇÃO DE SERVIÇOS =====

/// <summary>
/// Adiciona suporte para controladores MVC com vistas e força AUTENTICAÇÃO GLOBAL
/// </summary>
builder.Services.AddControllersWithViews(options => {
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

/// <summary>
/// Configura o contexto de base de dados com SQL Server
/// </summary>
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=(localdb)\\mssqllocaldb;Database=TrabalhoFinalDWEB2026;Trusted_Connection=True;MultipleActiveResultSets=true"));

/// <summary>
/// Configura o sistema de Autenticação e Autorização ASP.NET Core Identity
/// </summary>
builder.Services.AddDefaultIdentity<Utente>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole<string>>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// ===== SEEDING DE DADOS INICIAIS =====

using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<string>>>();
    var userManager = services.GetRequiredService<UserManager<Utente>>();
    // Injeta o contexto da base de dados para podermos criar os medicamentos
    var context = services.GetRequiredService<ApplicationDbContext>();

    // ===== 1. CRIAR ROLES =====
    string[] roles = { "Utente", "Doutor", "Farmaceuta" };
    foreach (var role in roles) {
        if (!await roleManager.RoleExistsAsync(role)) {
            await roleManager.CreateAsync(new IdentityRole<string> { Id = Guid.NewGuid().ToString(), Name = role });
        }
    }

    // ===== 2. CRIAR UTILIZADORES DE TESTE PADRÃO =====
    var defaultUsers = new[]
    {
        new { NumeroUtente = "000000001", Nome = "Utilizador Teste", Role = "Utente", Password = "Senha@123" },
        new { NumeroUtente = "000000002", Nome = "Médico Teste", Role = "Doutor", Password = "Senha@123" },
        new { NumeroUtente = "000000003", Nome = "Farmacêutico Teste", Role = "Farmaceuta", Password = "Senha@123" }
    };

    foreach (var userData in defaultUsers) {
        var existingUser = await userManager.FindByNameAsync(userData.NumeroUtente);
        if (existingUser == null) {
            var newUser = new Utente {
                Id = userData.NumeroUtente,
                UserName = userData.NumeroUtente,
                NumeroUtente = userData.NumeroUtente,
                Nome = userData.Nome,
                Email = $"{userData.NumeroUtente}@exemplo.com",
                DataNascimento = new DateTime(1990, 1, 1),
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(newUser, userData.Password);

            if (result.Succeeded) {
                await userManager.AddToRoleAsync(newUser, userData.Role);
            }
        }
    }

    // ===== 3. CRIAR MEDICAMENTOS DE TESTE PADRÃO =====
    // Verifica se a tabela de medicamentos está totalmente vazia antes de inserir
    if (!context.Medicamentos.Any()) {
        var medicamentos = new[]
        {
            new Medicamentos { Nome = "Paracetamol", Tipo = "Analgésico", Dosagem = "500mg" },
            new Medicamentos { Nome = "Ibuprofeno", Tipo = "Anti-inflamatório", Dosagem = "400mg" },
            new Medicamentos { Nome = "Amoxicilina", Tipo = "Antibiótico", Dosagem = "500mg" },
            new Medicamentos { Nome = "Dipirona", Tipo = "Analgésico", Dosagem = "500mg" },
            new Medicamentos { Nome = "Cetoconazol", Tipo = "Antifúngico", Dosagem = "200mg" },
            new Medicamentos { Nome = "Omeprazol", Tipo = "Antiácido", Dosagem = "20mg" },
            new Medicamentos { Nome = "Atorvastatina", Tipo = "Estatina", Dosagem = "10mg" },
            new Medicamentos { Nome = "Metformina", Tipo = "Antidiabético", Dosagem = "500mg" },
            new Medicamentos { Nome = "Losartano", Tipo = "Anti-hipertensivo", Dosagem = "50mg" },
            new Medicamentos { Nome = "Captopril", Tipo = "Anti-hipertensivo", Dosagem = "25mg" },
            new Medicamentos { Nome = "Sinvastatina", Tipo = "Estatina", Dosagem = "20mg" },
            new Medicamentos { Nome = "Ranitidina", Tipo = "Antiácido H2", Dosagem = "150mg" }
        };

        foreach (var med in medicamentos) {
            context.Medicamentos.Add(med);
        }

        await context.SaveChangesAsync();
    }
}

// ===== CONFIGURAÇÃO DO PIPELINE HTTP =====

if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 
app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapStaticAssets();

/// <summary>
/// Define a rota por defeito para MVC redirecionando para o LOGIN DO ACCOUNT CONTROLLER
/// </summary>
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

app.Run();
