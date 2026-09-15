using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.Dashboard {
    [Authorize(Roles = "Doutor")]
    public class DoutorBoardModel : PageModel {
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<DoutorBoardModel> _logger;

        public Utente? User { get; set; }

        public DoutorBoardModel(UserManager<Utente> userManager, ILogger<DoutorBoardModel> logger) {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task OnGetAsync() {
            User = await _userManager.GetUserAsync(HttpContext.User);
            _logger.LogInformation("Doutor {NumeroUtente} acedeu ao painel", User?.NumeroUtente);
        }
    }
}
