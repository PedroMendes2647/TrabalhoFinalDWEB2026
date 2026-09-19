using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.WebApp.Data;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.Farmaceuta {
    [Authorize(Roles = "Farmaceuta")]
    public class SearchModel : PageModel {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SearchModel> _logger;

        public List<Receita> SearchResults { get; set; } = [];

        public SearchModel(
            ApplicationDbContext context,
            ILogger<SearchModel> logger) {
            _context = context;
            _logger = logger;
        }

        public void OnGet() {
        }

        public async Task<IActionResult> OnPostAsync(string searchQuery) {
            if (string.IsNullOrWhiteSpace(searchQuery)) {
                ModelState.AddModelError("searchQuery", "Por favor, introduza um termo de pesquisa.");
                return Page();
            }

            List<Receita> receitas = [];

            // Tentar pesquisar por ID da receita
            if (int.TryParse(searchQuery, out int receitaId)) {
                var receita = await _context.Receitas
                    .Include(r => r.Utente)
                    .Include(r => r.ListaDeMedicamentos)
                        .ThenInclude(rm => rm.Medicamento)
                    .Include(r => r.DoutorUtente)
                    .Include(r => r.FarmaceutaUtente)
                    .FirstOrDefaultAsync(r => r.Id == receitaId);

                if (receita != null) {
                    receitas.Add(receita);
                }
            }

            // Pesquisar por número de utente
            var utenteReceitas = await _context.Receitas
                .Include(r => r.Utente)
                .Include(r => r.ListaDeMedicamentos)
                    .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.DoutorUtente)
                .Include(r => r.FarmaceutaUtente)
                .Where(r => r.Utente != null && r.Utente.NumeroUtente.Contains(searchQuery))
                .ToListAsync();

            receitas.AddRange(utenteReceitas);

            if (!receitas.Any()) {
                ModelState.AddModelError("searchQuery", "Nenhuma receita encontrada que corresponda à sua pesquisa.");
                return Page();
            }

            SearchResults = receitas.DistinctBy(r => r.Id).ToList();
            return RedirectToPage("SearchResults", new { query = searchQuery });
        }
    }
}
