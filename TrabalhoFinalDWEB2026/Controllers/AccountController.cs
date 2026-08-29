using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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

        [HttpGet]
        public IActionResult Register() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model) {
            if (ModelState.IsValid) {
                var age = DateTime.Today.Year - model.DataNascimento.Year;
                if (model.DataNascimento.Date > DateTime.Today.AddYears(-age)) {
                    age--;
                }

                if (age < 13) {
                    ModelState.AddModelError("DataNascimento", "Deve ter pelo menos 13 anos para se registar.");
                    return View(model);
                }

                var existingUser = await _userManager.FindByNameAsync(model.NumeroUtente);
                if (existingUser != null) {
                    ModelState.AddModelError("NumeroUtente", "Este Número de Utente já está registado.");
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
                    if (!await _roleManager.RoleExistsAsync("Utente")) {
                        await _roleManager.CreateAsync(new IdentityRole<string>("Utente"));
                    }
                    await _userManager.AddToRoleAsync(user, "Utente");

                    _logger.LogInformation("Novo utilizador registado: {NumeroUtente}", model.NumeroUtente);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login() {
            if (User.Identity?.IsAuthenticated ?? false) {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

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
                    _logger.LogInformation("Utilizador autenticado com sucesso: {NumeroUtente}", model.NumeroUtente);

                    // Alterado de ("Dashboard", "Utente") para ("Index", "Dashboard")
                    return RedirectToAction("Index", "Dashboard");
                }

                if (result.IsLockedOut) {
                    _logger.LogWarning("Conta bloqueada para: {NumeroUtente}", model.NumeroUtente);
                    ModelState.AddModelError(string.Empty, "Conta temporariamente bloqueada. Tente mais tarde.");
                    return View(model);
                }

                _logger.LogWarning("Falha no login para: {NumeroUtente}", model.NumeroUtente);
                ModelState.AddModelError(string.Empty, "Número de Utente ou palavra-passe incorretos.");
                return View(model);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Utilizador terminou a sessão.");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied() {
            return View();
        }
    }

    public class RegisterModel {
        [Required(ErrorMessage = "O número de utente é obrigatório.")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "O número de utente deve ter exatamente 9 dígitos.")]
        public string NumeroUtente { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [MinAge(13, ErrorMessage = "Deve ter pelo menos 13 anos para se registar.")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "As palavras-passe não coincidem.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginModel {
        [Required(ErrorMessage = "O número de utente é obrigatório.")]
        public string NumeroUtente { get; set; } = string.Empty;

        [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    public class MinAgeAttribute : ValidationAttribute {
        private readonly int _minAge;

        public MinAgeAttribute(int minAge) {
            _minAge = minAge;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext) {
            if (value is DateTime birthDate) {
                var age = DateTime.Today.Year - birthDate.Year;
                if (birthDate.Date > DateTime.Today.AddYears(-age)) {
                    age--;
                }

                if (age < _minAge) {
                    return new ValidationResult($"Deve ter pelo menos {_minAge} anos.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
