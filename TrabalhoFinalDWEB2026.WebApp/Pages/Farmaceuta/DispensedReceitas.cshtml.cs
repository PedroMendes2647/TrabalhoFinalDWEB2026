using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.WebApp.Data;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.Farmaceuta {
    [Authorize(Roles = "Farmaceuta")]
    public class DispensedReceitasModel : PageModel {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<DispensedReceitasModel> _logger;

        public List<Receita>? ReceitasAviadas { get; set; }

        public DispensedReceitasModel(
            ApplicationDbContext context,
            UserManager<Utente> userManager,
            ILogger<DispensedReceitasModel> logger) {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task OnGetAsync() {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) {
                return;
            }

            ReceitasAviadas = await _context.Receitas
                .Where(r => r.Estado == Receita.State.Aviada && r.FarmaceutaId == currentUser.Id)
                .Include(r => r.Utente)
                .Include(r => r.ListaDeMedicamentos)
                    .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.DoutorUtente)
                .Include(r => r.FarmaceutaUtente)
                .OrderByDescending(r => r.DataDispensacao)
                .ToListAsync();
        }
    }
}
