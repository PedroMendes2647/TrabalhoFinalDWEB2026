using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.WebApp.Data;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.Management {
    [Authorize]
    public class ViewReceitasModel : PageModel {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ViewReceitasModel> _logger;

        public List<Receita>? Receitas { get; set; }

        public ViewReceitasModel(
            ApplicationDbContext context,
            ILogger<ViewReceitasModel> logger) {
            _context = context;
            _logger = logger;
        }

        public async Task OnGetAsync() {
            Receitas = await _context.Receitas
                .Include(r => r.Utente)
                .Include(r => r.ListaDeMedicamentos)
                    .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.DoutorUtente)
                .Include(r => r.FarmaceutaUtente)
                .OrderByDescending(r => r.DataEmissao)
                .ToListAsync();

            _logger.LogInformation("Utilizador visualizou a lista de receitas");
        }
    }
}
