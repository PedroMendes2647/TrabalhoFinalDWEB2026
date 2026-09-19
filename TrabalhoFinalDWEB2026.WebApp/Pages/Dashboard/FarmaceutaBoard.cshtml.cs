using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.Dashboard {
    [Authorize(Roles = "Farmaceuta")]
    public class FarmaceutaBoardModel : PageModel {
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<FarmaceutaBoardModel> _logger;

        public Utente? User { get; set; }

        public FarmaceutaBoardModel(UserManager<Utente> userManager, ILogger<FarmaceutaBoardModel> logger) {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task OnGetAsync() {
            User = await _userManager.GetUserAsync(HttpContext.User);
            _logger.LogInformation("Farmacêutico {NumeroUtente} acedeu ao painel", User?.NumeroUtente);
        }
    }
}
