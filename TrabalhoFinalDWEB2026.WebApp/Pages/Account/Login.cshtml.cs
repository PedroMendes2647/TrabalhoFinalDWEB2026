using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.Account {
    [AllowAnonymous]
    public class LoginModel : PageModel {
        private readonly SignInManager<Utente> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<Utente> signInManager, ILogger<LoginModel> logger) {
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public void OnGet(string? returnUrl = null) {
            // Se o utilizador já está autenticado, redireciona para a página inicial
            if (User.Identity?.IsAuthenticated ?? false) {
                RedirectToPage("/Index");
            }

            ErrorMessage = null;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null) {
            if (ModelState.IsValid) {
                var result = await _signInManager.PasswordSignInAsync(
                    Input.NumeroUtente,
                    Input.Password,
                    Input.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded) {
                    _logger.LogInformation("Utilizador autenticado com sucesso: {NumeroUtente}", Input.NumeroUtente);
                    return RedirectToPage("/Dashboard/Index");
                }

                if (result.IsLockedOut) {
                    _logger.LogWarning("Conta bloqueada para: {NumeroUtente}", Input.NumeroUtente);
                    ErrorMessage = "Conta temporariamente bloqueada. Tente mais tarde.";
                    return Page();
                }

                _logger.LogWarning("Falha no login para: {NumeroUtente}", Input.NumeroUtente);
                ErrorMessage = "Número de Utente ou palavra-passe incorretos.";
                return Page();
            }

            return Page();
        }

        public class InputModel {
            [Required(ErrorMessage = "O número de utente é obrigatório.")]
            [Display(Name = "Número de Utente")]
            public string NumeroUtente { get; set; } = string.Empty;

            [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
            [DataType(DataType.Password)]
            [Display(Name = "Palavra-passe")]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Lembrar-me")]
            public bool RememberMe { get; set; }
        }
    }
}
