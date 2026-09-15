using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.WebApp.Data;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.Management {
    [Authorize]
    public class ViewMedicamentosModel : PageModel {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ViewMedicamentosModel> _logger;

        public List<Medicamentos>? Medicamentos { get; set; }

        public ViewMedicamentosModel(
            ApplicationDbContext context,
            ILogger<ViewMedicamentosModel> logger) {
            _context = context;
            _logger = logger;
        }

        public async Task OnGetAsync() {
            Medicamentos = await _context.Medicamentos
                .OrderBy(m => m.Nome)
                .ToListAsync();

            _logger.LogInformation("Utilizador visualizou a lista de medicamentos");
        }
    }
}
