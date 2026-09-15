using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.WebApp.Data;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.MyAccount {
    [Authorize]
    public class MyReceitasModel : PageModel {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<MyReceitasModel> _logger;

        public List<Receita>? Receitas { get; set; }

        public MyReceitasModel(
            ApplicationDbContext context,
            UserManager<Utente> userManager,
            ILogger<MyReceitasModel> logger) {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task OnGetAsync() {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) {
                return;
            }

            Receitas = await _context.Receitas
                .Where(r => r.UtenteId == currentUser.Id)
                .Include(r => r.ListaDeMedicamentos)
                    .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.DoutorUtente)
                .Include(r => r.FarmaceutaUtente)
                .OrderByDescending(r => r.DataEmissao)
                .ToListAsync();

            _logger.LogInformation("Utilizador {NumeroUtente} viu a lista de receitas pessoais",
                currentUser.NumeroUtente);
        }
    }
}
