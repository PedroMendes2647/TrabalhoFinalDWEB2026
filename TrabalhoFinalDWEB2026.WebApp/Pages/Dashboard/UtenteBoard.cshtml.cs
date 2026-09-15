using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.Dashboard {
    [Authorize(Roles = "Utente")]
    public class UtenteBoardModel : PageModel {
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<UtenteBoardModel> _logger;

        public Utente? User { get; set; }

        public UtenteBoardModel(UserManager<Utente> userManager, ILogger<UtenteBoardModel> logger) {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task OnGetAsync() {
            User = await _userManager.GetUserAsync(HttpContext.User);
            _logger.LogInformation("Utente {NumeroUtente} acedeu ao painel", User?.NumeroUtente);
        }
    }
}
