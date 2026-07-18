using Microsoft.AspNetCore.Authorization; // <-- ADICIONADO PARA O ALLOWANONYMOUS
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrabalhoFinalDWEB2026.Models;

namespace TrabalhoFinalDWEB2026.Controllers {
    [AllowAnonymous]
    public class AccountController : Controller {
        private readonly UserManager<Utente> _userManager;
        private readonly SignInManager<Utente> _signInManager;
        private readonly RoleManager<IdentityRole<string>> _roleManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<Utente> userManager,
            SignInManager<Utente> signInManager,
            RoleManager<IdentityRole<string>> roleManager,
            ILogger<AccountController> logger) {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        /// <summary>
        /// Apresenta o formulário de registo de novo utilizador
        /// </summary>
        [HttpGet]
        public IActionResult Register() {
            return View();
        }

        /// <summary>
        /// Processa o registo de um novo utilizador
        /// Verifica se o NumeroUtente já existe, cria o utilizador e atribui a role "Utente"
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model) {
            if (ModelState.IsValid) {
                // Verifica se NumeroUtente já existe
                var existingUser = await _userManager.FindByNameAsync(model.NumeroUtente);
                if (existingUser != null) {
                    ModelState.AddModelError("NumeroUtente", "Este Numero Utente já está registado.");
                    return View(model);
                }

                var user = new Utente {
                    Id = model.NumeroUtente,
                    UserName = model.NumeroUtente,
                    NumeroUtente = model.NumeroUtente,
                    Nome = model.Nome,
                    DataNascimento = model.DataNascimento,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded) {
                    // Atribui a role "Utente" a todos os novos registos
                    if (!await _roleManager.RoleExistsAsync("Utente")) {
                        await _roleManager.CreateAsync(new IdentityRole<string>("Utente"));
                    }
                    await _userManager.AddToRoleAsync(user, "Utente");

                    _logger.LogInformation("Novo utilizador criado com Numero Utente: {NumeroUtente}", model.NumeroUtente);

                    // Faz login automático após registo bem-sucedido
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        /// <summary>
        /// Apresenta o formulário de login
        /// </summary>
        [HttpGet]
        public IActionResult Login() {
            // Se o utilizador já está autenticado, redireciona para a página inicial
            if (User.Identity?.IsAuthenticated ?? false) {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        /// <summary>
        /// Processa o login do utilizador pelo NumeroUtente
        /// Suporta "Lembrar-me" e bloqueio de conta após múltiplas tentativas falhadas
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model) {
            if (ModelState.IsValid) {
                var result = await _signInManager.PasswordSignInAsync(
                    model.NumeroUtente,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded) {
                    _logger.LogInformation("Utilizador autenticado com Numero Utente: {NumeroUtente}", model.NumeroUtente);
                    return RedirectToAction("Index", "Dashboard");
                }

                if (result.IsLockedOut) {
                    _logger.LogWarning("Conta bloqueada para Numero Utente: {NumeroUtente}", model.NumeroUtente);
                    ModelState.AddModelError(string.Empty, "Conta bloqueada. Tente novamente mais tarde.");
                    return View(model);
                }

                _logger.LogWarning("Tentativa de login falhada para Numero Utente: {NumeroUtente}", model.NumeroUtente);
                ModelState.AddModelError(string.Empty, "Numero Utente ou senha inválidos.");
                return View(model);
            }

            return View(model);
        }

        /// <summary>
        /// Processa o logout do utilizador
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Utilizador fez logout.");
            return RedirectToAction("Index", "Home");
        }
    }

    public class RegisterModel {
        public string NumeroUtente { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginModel {
        public string NumeroUtente { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
