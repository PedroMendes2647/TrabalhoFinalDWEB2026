using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.WebApp.Data;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.Management {
    [Authorize]
    public class ReceitaDetailsModel : PageModel {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReceitaDetailsModel> _logger;

        public Receita? Receita { get; set; }

        public ReceitaDetailsModel(
            ApplicationDbContext context,
            ILogger<ReceitaDetailsModel> logger) {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int id) {
            Receita = await _context.Receitas
                .Include(r => r.Utente)
                .Include(r => r.ListaDeMedicamentos)
                    .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.DoutorUtente)
                .Include(r => r.FarmaceutaUtente)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (Receita == null) {
                return NotFound("Receita não encontrada.");
            }

            _logger.LogInformation("Visualização de receita {ReceitaId}", id);

            return Page();
        }
    }
}
