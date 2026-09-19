using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.WebApp.Data;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.MyAccount {
    [Authorize]
    public class ReceitaDetailsModel : PageModel {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<ReceitaDetailsModel> _logger;

        public Receita? Receita { get; set; }

        public ReceitaDetailsModel(
            ApplicationDbContext context,
            UserManager<Utente> userManager,
            ILogger<ReceitaDetailsModel> logger) {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int id) {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) {
                return RedirectToPage("/Account/Login");
            }

            Receita = await _context.Receitas
                .Include(r => r.ListaDeMedicamentos)
                    .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.DoutorUtente)
                .Include(r => r.FarmaceutaUtente)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (Receita == null) {
                return NotFound("Receita não encontrada.");
            }

            // Apenas o utente proprietário ou médicos/farmacêuticos podem visualizar
            var userRoles = await _userManager.GetRolesAsync(currentUser);
            if (Receita.UtenteId != currentUser.Id && 
                !userRoles.Contains("Doutor") && 
                !userRoles.Contains("Farmaceuta")) {
                return Forbid();
            }

            _logger.LogInformation("Utilizador {NumeroUtente} viu detalhes da receita {ReceitaId}",
                currentUser.NumeroUtente, id);

            return Page();
        }
    }
}
