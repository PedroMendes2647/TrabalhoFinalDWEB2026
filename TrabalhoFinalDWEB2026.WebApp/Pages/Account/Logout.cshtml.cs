using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.Account {
    [Authorize]
    public class LogoutModel : PageModel {
        private readonly SignInManager<Utente> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<Utente> signInManager, ILogger<LogoutModel> logger) {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null) {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Utilizador terminou a sessão.");
            return RedirectToPage("/Index");
        }
    }
}
