using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.WebApp.Data;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.Doutor {
    [Authorize(Roles = "Doutor")]
    public class ViewUtenteDateModel : PageModel {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<ViewUtenteDateModel> _logger;

        public Utente? Utente { get; set; }
        public List<Receita>? Receitas { get; set; }

        public ViewUtenteDateModel(
            ApplicationDbContext context,
            UserManager<Utente> userManager,
            ILogger<ViewUtenteDateModel> logger) {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string utenteId) {
            Utente = await _userManager.FindByIdAsync(utenteId);
            if (Utente == null) {
                return NotFound("Utilizador não encontrado.");
            }

            Receitas = await _context.Receitas
                .Where(r => r.UtenteId == utenteId)
                .Include(r => r.Utente)
                .Include(r => r.ListaDeMedicamentos)
                    .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.DoutorUtente)
                .Include(r => r.FarmaceutaUtente)
                .OrderByDescending(r => r.DataEmissao)
                .ToListAsync();

            _logger.LogInformation("Doutor visualizou dados do utente: {UtenteId}", utenteId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string utenteId, int receitaId) {
            var currentDoctor = await _userManager.GetUserAsync(User);
            if (currentDoctor == null) {
                return RedirectToPage("/Account/Login");
            }

            var receita = await _context.Receitas.FindAsync(receitaId);
            if (receita == null) {
                return NotFound("Receita não encontrada.");
            }

            // Verificar se o doutor que tenta eliminar é o que criou a receita
            if (receita.DoutorId != currentDoctor.Id) {
                return Forbid();
            }

            // Não permitir eliminar receitas já aviadas
            if (receita.Estado == Receita.State.Aviada) {
                ModelState.AddModelError("", "Não é permitido eliminar receitas já aviadas.");
                return await OnGetAsync(utenteId);
            }

            try {
                var medicamentosReceita = await _context.ReceitaMedicamentos
                    .Where(rm => rm.ReceitaId == receitaId)
                    .ToListAsync();

                _context.ReceitaMedicamentos.RemoveRange(medicamentosReceita);
                _context.Receitas.Remove(receita);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Doutor {DoutorId} eliminou receita {ReceitaId}", currentDoctor.Id, receitaId);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Erro ao eliminar receita {ReceitaId}", receitaId);
            }

            return await OnGetAsync(utenteId);
        }
    }
}
