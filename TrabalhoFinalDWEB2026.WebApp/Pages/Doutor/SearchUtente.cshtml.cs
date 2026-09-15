using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrabalhoFinalDWEB2026.WebApp.Data;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.Doutor {
    [Authorize(Roles = "Doutor")]
    public class SearchUtenteModel : PageModel {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<SearchUtenteModel> _logger;

        public SearchUtenteModel(
            ApplicationDbContext context,
            UserManager<Utente> userManager,
            ILogger<SearchUtenteModel> logger) {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public void OnGet() {
            // Página de pesquisa de utente
        }

        public async Task<IActionResult> OnPostAsync(string numeroUtente) {
            if (string.IsNullOrWhiteSpace(numeroUtente)) {
                ModelState.AddModelError("numeroUtente", "Por favor, introduza um Número de Utente.");
                return Page();
            }

            var utente = await _userManager.FindByNameAsync(numeroUtente);
            if (utente == null) {
                ModelState.AddModelError("numeroUtente", "Utilizador não encontrado.");
                return Page();
            }

            _logger.LogInformation("Doutor pesquisou utente: {NumeroUtente}", numeroUtente);
            return RedirectToPage("ViewUtenteData", new { utenteId = utente.Id });
        }
    }
}
