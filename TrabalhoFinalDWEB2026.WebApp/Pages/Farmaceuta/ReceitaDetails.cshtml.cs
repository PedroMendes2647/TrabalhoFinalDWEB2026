using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.WebApp.Data;
using TrabalhoFinalDWEB2026.WebApp.Hubs;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.Farmaceuta {
    [Authorize(Roles = "Farmaceuta")]
    public class ReceitaDetailsModel : PageModel {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<ReceitaDetailsModel> _logger;
        private readonly IHubContext<ReceitaHub> _hubContext;

        public Receita? Receita { get; set; }

        public ReceitaDetailsModel(
            ApplicationDbContext context,
            UserManager<Utente> userManager,
            ILogger<ReceitaDetailsModel> logger,
            IHubContext<ReceitaHub> hubContext) {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _hubContext = hubContext;
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

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id) {
            var receita = await _context.Receitas
                .Include(r => r.Utente)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receita == null) {
                return NotFound("Receita não encontrada.");
            }

            if (receita.Estado != Receita.State.Emitida) {
                ModelState.AddModelError(string.Empty,
                    $"Não consegue dispensar receita com o estado '{receita.Estado}'. Só receitas em estado 'Emitida' podem ser dispensadas.");
                return await OnGetAsync(id);
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

            // Disparar notificação via SignalR em tempo real
            await _hubContext.Clients.All.SendAsync(
                "NotificacaoReceita",
                $"A receita #{receita.Id} de {receita.Utente?.Nome} foi dispensada com sucesso.",
                "Receita Aviada"
            );

            // Aguardar 2 segundos para permitir que o toast seja exibido antes do redirect
            await Task.Delay(2000);

            return RedirectToPage("Dashboard");
        }
    }
}
