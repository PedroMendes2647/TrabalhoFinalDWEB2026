using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.Dashboard {
    [Authorize]
    public class IndexModel : PageModel {
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(UserManager<Utente> userManager, ILogger<IndexModel> logger) {
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Redireciona para o painel apropriado com base na função do utilizador
        /// </summary>
        public async Task<IActionResult> OnGetAsync() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return RedirectToPage("/Account/Login");
            }

            var roles = await _userManager.GetRolesAsync(user);
            _logger.LogInformation("Utilizador {NumeroUtente} tem funções: {Roles}", user.NumeroUtente, string.Join(", ", roles));

            // Verifica cada função e redireciona para o painel apropriado
            if (roles.Contains("Doutor")) {
                return RedirectToPage("DoutorBoard");
            }
            else if (roles.Contains("Farmaceuta")) {
                return RedirectToPage("FarmaceutaBoard");
            }
            else if (roles.Contains("Utente")) {
                return RedirectToPage("UtenteBoard");
            }

            // Fallback padrão
            return RedirectToPage("UtenteBoard");
        }
    }
}
