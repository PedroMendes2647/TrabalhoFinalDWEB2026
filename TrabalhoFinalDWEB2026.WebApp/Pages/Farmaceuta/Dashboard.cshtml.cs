using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.WebApp.Data;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.Farmaceuta {
    [Authorize(Roles = "Farmaceuta")]
    public class DashboardModel : PageModel {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<DashboardModel> _logger;

        public List<Receita>? ReceitasEmitidas { get; set; }

        public DashboardModel(
            ApplicationDbContext context,
            UserManager<Utente> userManager,
            ILogger<DashboardModel> logger) {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task OnGetAsync() {
            ReceitasEmitidas = await _context.Receitas
                .Where(r => r.Estado == Receita.State.Emitida)
                .Include(r => r.Utente)
                .Include(r => r.ListaDeMedicamentos)
                    .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.DoutorUtente)
                .OrderByDescending(r => r.DataEmissao)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDispenseAsync(int id) {
            var receita = await _context.Receitas.FindAsync(id);

            if (receita == null) {
                return NotFound("Receita não encontrada.");
            }

            if (receita.Estado != Receita.State.Emitida) {
                ModelState.AddModelError(string.Empty,
                    $"Não consegue dispensar receita com o estado '{receita.Estado}'. Só receitas em estado 'Emitida' podem ser dispensadas.");
                return RedirectToPage("ReceitaDetails", new { id });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) {
                return RedirectToPage("/Account/Login");
            }

            receita.Estado = Receita.State.Aviada;
            receita.FarmaceutaId = currentUser.Id;
            receita.DataDispensacao = DateTime.Now;

            _context.Receitas.Update(receita);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Farmacêutico {FarmaceutaId} dispensou a receita {ReceitaId}",
                currentUser.Id, id);

            return RedirectToPage("Dashboard");
        }
    }
}
