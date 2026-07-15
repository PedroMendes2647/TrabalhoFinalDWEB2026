using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.Data;
using TrabalhoFinalDWEB2026.Models;

namespace TrabalhoFinalDWEB2026.Controllers
{
    [Authorize(Roles = "Farmaceuta")]
    public class FarmaceutaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<FarmaceutaController> _logger;

        public FarmaceutaController(
            ApplicationDbContext context,
            UserManager<Utente> userManager,
            ILogger<FarmaceutaController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // Pharmacist dashboard showing receitas to be dispensed
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            // Show all receitas that are in "Emitida" status (awaiting dispensing)
            var receitasEmitidas = await _context.Receitas
                .Where(r => r.Estado == "Emitida")
                .Include(r => r.Utente)
                .Include(r => r.ListaDeMedicamentos)
                .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.Doutor)
                .OrderByDescending(r => r.DataEmissao)
                .ToListAsync();

            return View(receitasEmitidas);
        }

        // View receita details
        [HttpGet]
        public async Task<IActionResult> ReceitaDetails(int id)
        {
            var receita = await _context.Receitas
                .Include(r => r.Utente)
                .Include(r => r.ListaDeMedicamentos)
                .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.Doutor)
                .Include(r => r.Farmaceuta)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receita == null)
            {
                return NotFound("Receita not found.");
            }

            return View(receita);
        }

        // Change receita status from "Emitida" to "Aviada"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DispenseReceita(int id)
        {
            var receita = await _context.Receitas.FindAsync(id);

            if (receita == null)
            {
                return NotFound("Receita not found.");
            }

            if (receita.Estado != "Emitida")
            {
                ModelState.AddModelError(string.Empty, $"Cannot dispense a receita with status '{receita.Estado}'. Only 'Emitida' receitas can be dispensed.");
                return RedirectToAction("ReceitaDetails", new { id });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var currentFarmaceuta = await _context.Farmaceutas
                .FirstOrDefaultAsync(f => f.Id == currentUser.Id);

            if (currentFarmaceuta == null)
            {
                _logger.LogWarning("User with role Farmaceuta is not a Farmaceuta entity. UserId: {UserId}", currentUser.Id);
                return Forbid("You must be registered as a Farmaceuta to dispense receitas.");
            }

            receita.Estado = "Aviada";
            receita.FarmaceutaId = currentUser.Id;
            receita.DataDispensacao = DateTime.Now;

            _context.Receitas.Update(receita);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Farmaceuta {FarmaceutaId} dispensed receita {ReceitaId}", currentUser.Id, id);

            return RedirectToAction("Dashboard");
        }

        // View all dispended receitas
        [HttpGet]
        public async Task<IActionResult> DispensedReceitas()
        {
            var receitasAviadas = await _context.Receitas
                .Where(r => r.Estado == "Aviada")
                .Include(r => r.Utente)
                .Include(r => r.ListaDeMedicamentos)
                .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.Doutor)
                .Include(r => r.Farmaceuta)
                .OrderByDescending(r => r.DataDispensacao)
                .ToListAsync();

            return View(receitasAviadas);
        }

        // Search for a receita by numero utente or receita id
        [HttpGet]
        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                ModelState.AddModelError("searchQuery", "Please enter a search term.");
                return View();
            }

            List<Receita> receitas = new List<Receita>();

            // Try to search by receita ID
            if (int.TryParse(searchQuery, out int receitaId))
            {
                var receita = await _context.Receitas
                    .Include(r => r.Utente)
                    .Include(r => r.ListaDeMedicamentos)
                    .ThenInclude(rm => rm.Medicamento)
                    .Include(r => r.Doutor)
                    .Include(r => r.Farmaceuta)
                    .FirstOrDefaultAsync(r => r.Id == receitaId);

                if (receita != null)
                {
                    receitas.Add(receita);
                }
            }

            // Also search by Numero Utente
            var utenteReceitas = await _context.Receitas
                .Include(r => r.Utente)
                .Include(r => r.ListaDeMedicamentos)
                .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.Doutor)
                .Include(r => r.Farmaceuta)
                .Where(r => r.Utente.NumeroUtente.Contains(searchQuery))
                .ToListAsync();

            receitas.AddRange(utenteReceitas);

            if (!receitas.Any())
            {
                ModelState.AddModelError("searchQuery", "No receitas found matching your search.");
                return View();
            }

            return View("SearchResults", receitas);
        }
    }
}
