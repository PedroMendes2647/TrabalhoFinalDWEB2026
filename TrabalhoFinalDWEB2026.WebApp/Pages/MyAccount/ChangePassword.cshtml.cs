using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.MyAccount {
    [Authorize]
    public class ChangePasswordModel : PageModel {
        private readonly UserManager<Utente> _userManager;
        private readonly SignInManager<Utente> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;

        [BindProperty]
        public string CurrentPassword { get; set; } = string.Empty;

        [BindProperty]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        public ChangePasswordModel(
            UserManager<Utente> userManager,
            SignInManager<Utente> signInManager,
            ILogger<ChangePasswordModel> logger) {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public void OnGet() {
        }

        public async Task<IActionResult> OnPostAsync() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return RedirectToPage("/Account/Login");
            }

            if (!ModelState.IsValid) {
                return Page();
            }

            if (NewPassword != ConfirmPassword) {
                ModelState.AddModelError(string.Empty, "As palavras-passe não coincidem.");
                return Page();
            }

            var result = await _userManager.ChangePasswordAsync(user, CurrentPassword, NewPassword);
            if (result.Succeeded) {
                _logger.LogInformation("Utilizador {NumeroUtente} alterou a palavra-passe",
                    user.NumeroUtente);

                // Fazer logout do utilizador
                await _signInManager.SignOutAsync();
                return RedirectToPage("/Account/Login", new { changedPassword = true });
            }

            foreach (var error in result.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
