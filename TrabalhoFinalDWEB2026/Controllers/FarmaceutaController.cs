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

        /// <summary>
        /// Painel de controlo do farmacêutico
        /// Mostra todas as receitas que ainda não foram dispensadas (estado "Emitida")
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            // Obtém todas as receitas ainda não dispensadas
            var receitasEmitidas = await _context.Receitas
                .Where(r => r.Estado == "Emitida")
                .Include(r => r.Utente)
                .Include(r => r.ListaDeMedicamentos)
                .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.DoutorUtente)
                .OrderByDescending(r => r.DataEmissao)
                .ToListAsync();

            return View(receitasEmitidas);
        }

        /// <summary>
        /// Visualiza os detalhes de uma receita específica
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ReceitaDetails(int id)
        {
            var receita = await _context.Receitas
                .Include(r => r.Utente)
                .Include(r => r.ListaDeMedicamentos)
                .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.DoutorUtente)
                .Include(r => r.FarmaceutaUtente)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receita == null)
            {
                return NotFound("Receita não encontrada.");
            }

            return View(receita);
        }

        /// <summary>
        /// Altera o estado da receita de "Emitida" para "Aviada"
        /// Registra qual farmacêutico efetuou a dispensação e a data
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DispenseReceita(int id)
        {
            var receita = await _context.Receitas.FindAsync(id);

            if (receita == null)
            {
                return NotFound("Receita não encontrada.");
            }

            if (receita.Estado != "Emitida")
            {
                ModelState.AddModelError(string.Empty, $"Não consegue dispensar receita com o estado '{receita.Estado}'. Só receitas em estado 'Emitida' podem ser dispensadas.");
                return RedirectToAction("ReceitaDetails", new { id });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var currentFarmaceuta = await _context.Farmaceutas
                .FirstOrDefaultAsync(f => f.Id == currentUser.Id);

            if (currentFarmaceuta == null)
            {
                _logger.LogWarning("Utilizador com Id {UserId} não está registado como Farmaceuta.", currentUser.Id);
                return Forbid("Precisa de estar registado como Farmaceuta para dispensar receitas.");
            }

            receita.Estado = "Aviada";
            receita.FarmaceutaId = currentUser.Id;
            receita.DataDispensacao = DateTime.Now;

            _context.Receitas.Update(receita);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Farmacêutico {FarmaceutaId} dispensou a receita {ReceitaId}", currentUser.Id, id);

            return RedirectToAction("Dashboard");
        }

        /// <summary>
        /// Lista todas as receitas já dispensadas (estado "Aviada")
        /// Ordenadas por data de dispensação mais recente
        /// Apenas receitas aviadas por este farmacêutico
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DispensedReceitas()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var receitasAviadas = await _context.Receitas
                .Where(r => r.Estado == "Aviada" && r.FarmaceutaId == currentUser.Id)
                .Include(r => r.Utente)
                .Include(r => r.ListaDeMedicamentos)
                .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.DoutorUtente)
                .Include(r => r.FarmaceutaUtente)
                .OrderByDescending(r => r.DataDispensacao)
                .ToListAsync();

            return View(receitasAviadas);
        }

        /// <summary>
        /// Apresenta o formulário para pesquisar receitas
        /// </summary>
        [HttpGet]
        public IActionResult Search()
        {
            return View();
        }

        /// <summary>
        /// Processa a pesquisa de receitas por ID ou número de utilizador
        /// Permite pesquisar tanto por ID da receita como por NumeroUtente do paciente
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                ModelState.AddModelError("searchQuery", "Entre com um termo de busca.");
                return View();
            }

            List<Receita> receitas = new List<Receita>();

            // Pesquisa por ID da receita
            if (int.TryParse(searchQuery, out int receitaId))
            {
                var receita = await _context.Receitas
                    .Include(r => r.Utente)
                    .Include(r => r.ListaDeMedicamentos)
                    .ThenInclude(rm => rm.Medicamento)
                    .Include(r => r.DoutorUtente)
                    .Include(r => r.FarmaceutaUtente)
                    .FirstOrDefaultAsync(r => r.Id == receitaId);

                if (receita != null)
                {
                    receitas.Add(receita);
                }
            }

            // Pesquisa por número de utilizador
            var utenteReceitas = await _context.Receitas
                .Include(r => r.Utente)
                .Include(r => r.ListaDeMedicamentos)
                .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.DoutorUtente)
                .Include(r => r.FarmaceutaUtente)
                .Where(r => r.Utente.NumeroUtente.Contains(searchQuery))
                .ToListAsync();

            receitas.AddRange(utenteReceitas);

            if (!receitas.Any())
            {
                ModelState.AddModelError("searchQuery", "Nenhuma receita encontrada que corresponda à sua pesquisa.");
                return View();
            }

            return View("SearchResults", receitas);
        }
    }
}
