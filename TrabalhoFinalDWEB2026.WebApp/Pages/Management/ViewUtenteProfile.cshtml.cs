using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.WebApp.Data;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.Management {
    [Authorize]
    public class ViewUtenteProfileModel : PageModel {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<ViewUtenteProfileModel> _logger;

        public Utente? Utente { get; set; }
        public int ReceitasCount { get; set; }

        public ViewUtenteProfileModel(
            ApplicationDbContext context,
            UserManager<Utente> userManager,
            ILogger<ViewUtenteProfileModel> logger) {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string? id) {
            if (string.IsNullOrEmpty(id)) {
                return BadRequest("ID do utente é obrigatório.");
            }

            Utente = await _userManager.FindByIdAsync(id);

            if (Utente == null) {
                return NotFound("Utente não encontrado.");
            }

            // Contar receitas do utente
            ReceitasCount = await _context.Receitas
                .Where(r => r.UtenteId == Utente.Id)
                .CountAsync();

            _logger.LogInformation("Visualização de perfil do utente {UtenteId}", id);

            return Page();
        }
    }
}
