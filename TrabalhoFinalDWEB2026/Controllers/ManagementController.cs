using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.Data;
using TrabalhoFinalDWEB2026.Models;

namespace TrabalhoFinalDWEB2026.Controllers {
    [Authorize]
    public class ManagementController : Controller {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<ManagementController> _logger;

        public ManagementController(
            ApplicationDbContext context,
            UserManager<Utente> userManager,
            ILogger<ManagementController> logger) {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Ver receitas aviadas (dispensadas) - apenas para farmacêuticos
        /// </summary>
        public async Task<IActionResult> ViewAviacoes() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var userRoles = await _userManager.GetRolesAsync(user);

            if (!userRoles.Contains("Farmaceuta")) {
                return RedirectToAction("Index", "Dashboard");
            }

            var receitas = await _context.Receitas
                .Include(r => r.ListaDeMedicamentos)
                .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.Utente)
                .Include(r => r.DoutorUtente)
                .Include(r => r.FarmaceutaUtente)
                .Where(r => r.Estado == Receita.State.Aviada) // Atualizado para Enum
                .OrderByDescending(r => r.DataDispensacao)
                .ToListAsync();

            _logger.LogInformation("Utilizador {NumeroUtente} viu a lista de receitas aviadas", user.NumeroUtente);
            return View(receitas);
        }

        /// <summary>
        /// Ver receitas pendentes de aviamento ou do próprio utilizador
        /// </summary>
        public async Task<IActionResult> ViewReceitas(string numeroUtente = "") {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var userRoles = await _userManager.GetRolesAsync(user);
            IEnumerable<Receita> receipts = new List<Receita>();

            if (userRoles.Contains("Utente") && !userRoles.Contains("Doutor") && !userRoles.Contains("Farmaceuta")) {
                receipts = await _context.Receitas
                    .Where(r => r.UtenteId == user.Id)
                    .Include(r => r.ListaDeMedicamentos)
                    .ThenInclude(rm => rm.Medicamento)
                    .Include(r => r.Utente)
                    .Include(r => r.DoutorUtente)
                    .Include(r => r.FarmaceutaUtente)
                    .OrderByDescending(r => r.DataEmissao)
                    .ToListAsync();
            } else if (userRoles.Contains("Doutor")) {
                receipts = await _context.Receitas
                    .Where(r => r.DoutorId == user.Id)
                    .Include(r => r.ListaDeMedicamentos)
                    .ThenInclude(rm => rm.Medicamento)
                    .Include(r => r.Utente)
                    .Include(r => r.DoutorUtente)
                    .Include(r => r.FarmaceutaUtente)
                    .OrderByDescending(r => r.DataEmissao)
                    .ToListAsync();
            } else if (userRoles.Contains("Farmaceuta")) {
                if (!string.IsNullOrWhiteSpace(numeroUtente)) {
                    var query = _context.Receitas
                        .Include(r => r.ListaDeMedicamentos)
                        .ThenInclude(rm => rm.Medicamento)
                        .Include(r => r.Utente)
                        .Include(r => r.DoutorUtente)
                        .Include(r => r.FarmaceutaUtente)
                        .Where(r => r.Estado == Receita.State.Emitida) // Atualizado para Enum
                        .Where(r => r.Utente != null && r.Utente.NumeroUtente.Contains(numeroUtente));

                    receipts = await query
                        .OrderByDescending(r => r.DataEmissao)
                        .ToListAsync();

                    ViewBag.NumeroUtentePesquisado = numeroUtente;
                } else {
                    receipts = new List<Receita>();
                    ViewBag.NumeroUtentePesquisado = "";
                }
            }

            _logger.LogInformation("Utilizador {NumeroUtente} viu a lista de receitas", user.NumeroUtente);
            return View(receipts);
        }

        /// <summary>
        /// Ver todos os medicamentos disponíveis
        /// </summary>
        public async Task<IActionResult> ViewMedicamentos() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var medications = await _context.Medicamentos.ToListAsync();

            _logger.LogInformation("Utilizador {NumeroUtente} viu a lista de medicamentos", user.NumeroUtente);
            return View(medications);
        }

        /// <summary>
        /// Ver lista de utentes (apenas para médicos e farmacêuticos)
        /// </summary>
        [Authorize(Roles = "Doutor,Farmaceuta")]
        public async Task<IActionResult> ViewUtentes() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var utentes = await _context.Utentes.ToListAsync();

            _logger.LogInformation("Utilizador {NumeroUtente} viu a lista de utentes", user.NumeroUtente);
            return View(utentes);
        }

        /// <summary>
        /// Editar perfil do utilizador
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EditPerfil() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            return View(user);
        }

        /// <summary>
        /// Atualizar informações do perfil
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPerfil(Utente model) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            if (user.Id != model.Id)
                return Forbid();

            user.Nome = model.Nome;
            user.Email = model.Email;
            user.DataNascimento = model.DataNascimento;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded) {
                _logger.LogInformation("Utilizador {NumeroUtente} atualizou o perfil", user.NumeroUtente);
                return RedirectToAction("Index", "Dashboard");
            }

            return View(user);
        }

        /// <summary>
        /// Página para alterar palavra-passe
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ChangePassword() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        /// <summary>
        /// Processar alteração de palavra-passe
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded) {
                _logger.LogInformation("Utilizador {NumeroUtente} alterou a palavra-passe", user.NumeroUtente);
                return RedirectToAction("Index", "Dashboard");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        /// <summary>
        /// Visualiza o perfil de um utente específico
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ViewUtenteProfile(string utenteId) {
            if (string.IsNullOrEmpty(utenteId))
                return NotFound("Utente não encontrado.");

            var utente = await _userManager.FindByIdAsync(utenteId);
            if (utente == null)
                return NotFound("Utente não encontrado.");

            _logger.LogInformation("Utilizador visualizou perfil de {NumeroUtente}", utente.NumeroUtente);
            return View(utente);
        }

        /// <summary>
        /// Visualiza os detalhes de uma receita específica
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ReceitaDetails(int receitaId) {
            var receita = await _context.Receitas
                .Include(r => r.Utente)
                .Include(r => r.DoutorUtente)
                .Include(r => r.FarmaceutaUtente)
                .Include(r => r.ListaDeMedicamentos)
                    .ThenInclude(rm => rm.Medicamento)
                .FirstOrDefaultAsync(r => r.Id == receitaId);

            if (receita == null)
                return NotFound("Receita não encontrada.");

            _logger.LogInformation("Visualização de receita {ReceitaId}", receitaId);
            return View(receita);
        }

        /// <summary>
        /// Marca uma receita como aviada (dispensada) pelo farmacêutico
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DispenseReceita(int id) {
            var receita = await _context.Receitas.FindAsync(id);

            if (receita == null)
                return NotFound("Receita não encontrada.");

            if (receita.Estado != Receita.State.Emitida) // Atualizado para Enum
            {
                ModelState.AddModelError(string.Empty, $"Não consegue dispensar receita com o estado '{receita.Estado}'. Só receitas em estado 'Emitida' podem ser dispensadas.");
                return RedirectToAction("ReceitaDetails", new { receitaId = id });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return RedirectToAction("Login", "Account");

            var userRoles = await _userManager.GetRolesAsync(currentUser);

            if (!userRoles.Contains("Farmaceuta")) {
                _logger.LogWarning("Utilizador com Id {UserId} não tem permissão Farmaceuta.", currentUser.Id);
                return Forbid();
            }

            receita.Estado = Receita.State.Aviada; // Atualizado para Enum
            receita.FarmaceutaId = currentUser.Id;
            receita.DataDispensacao = DateTime.Now;

            _context.Receitas.Update(receita);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Farmacêutico {FarmaceutaId} dispensou a receita {ReceitaId}", currentUser.Id, id);

            return RedirectToAction("ReceitaDetails", new { receitaId = id });
        }
    }

    public class ChangePasswordModel {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
