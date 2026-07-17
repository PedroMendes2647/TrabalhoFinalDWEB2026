using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.Data;
using TrabalhoFinalDWEB2026.Models;

var builder = WebApplication.CreateBuilder(args);

// ===== CONFIGURAÇÃO DE SERVIÇOS =====

/// <summary>
/// Adiciona suporte para controladores MVC com vistas
/// </summary>

builder.Services.AddControllersWithViews();

/// <summary>
/// Configura o contexto de base de dados com SQL Server
/// </summary>
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=(localdb)\\mssqllocaldb;Database=TrabalhoFinalDWEB2026;Trusted_Connection=True;MultipleActiveResultSets=true"));

/// <summary>
/// Configura o sistema de Autenticação e Autorização ASP.NET Core Identity
/// - Entidade de utilizador: Utente
/// - Desativa confirmação de email obrigatória
/// - Suporte para roles baseado em string
/// - Armazenamento em Entity Framework Core
/// </summary>
builder.Services.AddDefaultIdentity<Utente>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole<string>>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// ===== SEEDING DE DADOS INICIAIS =====

/// <summary>
/// Inicializa as roles da aplicação no startup
/// Cria as roles: Utente, Doutor, Farmaceuta se não existirem
/// Cria os utilizadores de teste padrão com números de utilizador:
/// - 000000001: Utilizador (Role: Utente)
/// - 000000002: Médico (Role: Doutor)
/// - 000000003: Farmacêutico (Role: Farmaceuta)
/// </summary>
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<string>>>();
    var userManager = services.GetRequiredService<UserManager<Utente>>();

    // ===== CRIAR ROLES =====
    string[] roles = { "Utente", "Doutor", "Farmaceuta" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<string> { Id = Guid.NewGuid().ToString(), Name = role });
        }
    }

    // ===== CRIAR UTILIZADORES DE TESTE PADRÃO =====

    // Dados dos utilizadores de teste
    var defaultUsers = new[]
    {
        new { NumeroUtente = "000000001", Nome = "Utilizador Teste", Role = "Utente", Password = "Senha@123" },
        new { NumeroUtente = "000000002", Nome = "Médico Teste", Role = "Doutor", Password = "Senha@123" },
        new { NumeroUtente = "000000003", Nome = "Farmacêutico Teste", Role = "Farmaceuta", Password = "Senha@123" }
    };

    foreach (var userData in defaultUsers)
    {
        // Verifica se o utilizador já existe
        var existingUser = await userManager.FindByNameAsync(userData.NumeroUtente);
        if (existingUser == null)
        {
            // Cria novo utilizador
            var newUser = new Utente
            {
                Id = userData.NumeroUtente,
                UserName = userData.NumeroUtente,
                NumeroUtente = userData.NumeroUtente,
                Nome = userData.Nome,
                Email = $"{userData.NumeroUtente}@exemplo.com",
                DataNascimento = new DateTime(1990, 1, 1),
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(newUser, userData.Password);

            if (result.Succeeded)
            {
                // Atribui a role ao utilizador
                await userManager.AddToRoleAsync(newUser, userData.Role);
            }
        }
    }
}

// ===== CONFIGURAÇÃO DO PIPELINE HTTP =====

/// <summary>
/// Configuração do pipeline de requisição HTTP
/// </summary>
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

/// <summary>
/// Redireciona requisições HTTP para HTTPS
/// </summary>
app.UseHttpsRedirection();

/// <summary>
/// Ativa o sistema de routing da aplicação
/// </summary>
app.UseRouting();

/// <summary>
/// Ativa o middleware de autenticação e autorização
/// Deve ser colocado antes de MapControllers e MapRazorPages
/// </summary>
app.UseAuthorization();

/// <summary>
/// Mapeia os controladores API e MVC
/// Permite que requisições cheguem aos controladores decorados com [ApiController] e [Controller]
/// </summary>
app.MapControllers();

/// <summary>
/// Mapeia ficheiros estáticos (CSS, JS, imagens, etc.)
/// </summary>
app.MapStaticAssets();

/// <summary>
/// Define a rota por defeito para MVC redirecionando para o Index da Home
/// </summary>
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

/// <summary>
/// Mapeia as páginas Razor (se utilizadas no projeto)
/// </summary>
app.MapRazorPages();

/// <summary>
/// Inicia a aplicação e fica à escuta de requisições
/// </summary>
app.Run();
