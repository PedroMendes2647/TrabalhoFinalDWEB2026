using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrabalhoFinalDWEB2026.Models;

namespace TrabalhoFinalDWEB2026.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Utente> _userManager;
        private readonly SignInManager<Utente> _signInManager;
        private readonly RoleManager<IdentityRole<string>> _roleManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<Utente> userManager,
            SignInManager<Utente> signInManager,
            RoleManager<IdentityRole<string>> roleManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if NumeroUtente already exists
                var existingUser = await _userManager.FindByNameAsync(model.NumeroUtente);
                if (existingUser != null)
                {
                    ModelState.AddModelError("NumeroUtente", "This Numero Utente is already registered.");
                    return View(model);
                }

                var user = new Utente
                {
                    UserName = model.NumeroUtente,
                    NumeroUtente = model.NumeroUtente,
                    Nome = model.Nome,
                    DataNascimento = model.DataNascimento,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Assign the "Utente" role to all new registrations
                    if (!await _roleManager.RoleExistsAsync("Utente"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole<string>("Utente"));
                    }
                    await _userManager.AddToRoleAsync(user, "Utente");

                    _logger.LogInformation("User created a new account with Numero Utente: {NumeroUtente}", model.NumeroUtente);

                    // Sign in after successful registration
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.NumeroUtente,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in with Numero Utente: {NumeroUtente}", model.NumeroUtente);
                    return RedirectToAction("Dashboard", "Utente");
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out for Numero Utente: {NumeroUtente}", model.NumeroUtente);
                    ModelState.AddModelError(string.Empty, "Account locked. Try again later.");
                    return View(model);
                }

                _logger.LogWarning("Failed login attempt for Numero Utente: {NumeroUtente}", model.NumeroUtente);
                ModelState.AddModelError(string.Empty, "Invalid Numero Utente or password.");
                return View(model);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
        }
    }

    public class RegisterModel
    {
        public string NumeroUtente { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginModel
    {
        public string NumeroUtente { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
