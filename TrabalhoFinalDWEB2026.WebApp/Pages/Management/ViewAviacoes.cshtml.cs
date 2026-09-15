using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.WebApp.Data;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.Management {
    [Authorize]
    public class ViewAviacoesModel : PageModel {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ViewAviacoesModel> _logger;

        public List<Receita>? ReceitasAviadas { get; set; }

        public ViewAviacoesModel(
            ApplicationDbContext context,
            ILogger<ViewAviacoesModel> logger) {
            _context = context;
            _logger = logger;
        }

        public async Task OnGetAsync() {
            ReceitasAviadas = await _context.Receitas
                .Where(r => r.Estado == Receita.State.Aviada)
                .Include(r => r.Utente)
                .Include(r => r.ListaDeMedicamentos)
                    .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.DoutorUtente)
                .Include(r => r.FarmaceutaUtente)
                .OrderByDescending(r => r.DataDispensacao)
                .ToListAsync();

            _logger.LogInformation("Utilizador visualizou a lista de receitas aviadas");
        }
    }
}
