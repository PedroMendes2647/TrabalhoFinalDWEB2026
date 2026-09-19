using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.MyAccount {
    [Authorize]
    public class MyDataModel : PageModel {
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<MyDataModel> _logger;

        public Utente? CurrentUser { get; set; }

        public MyDataModel(
            UserManager<Utente> userManager,
            ILogger<MyDataModel> logger) {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task OnGetAsync() {
            CurrentUser = await _userManager.GetUserAsync(User);
            if (CurrentUser != null) {
                _logger.LogInformation("Utilizador {NumeroUtente} viu a página de dados pessoais",
                    CurrentUser.NumeroUtente);
            }
        }
    }
}
