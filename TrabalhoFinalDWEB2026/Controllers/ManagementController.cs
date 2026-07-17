using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.Data;
using TrabalhoFinalDWEB2026.Models;

namespace TrabalhoFinalDWEB2026.Controllers
{
    [Authorize]
    public class ManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<ManagementController> _logger;

        public ManagementController(
            ApplicationDbContext context,
            UserManager<Utente> userManager,
            ILogger<ManagementController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Ver todas as receitas baseado na função do utilizador
        /// </summary>
        public async Task<IActionResult> ViewReceitas()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var userRoles = await _userManager.GetRolesAsync(user);
            IEnumerable<Receita> receipts = new List<Receita>();

            if (userRoles.Contains("Utente"))
            {
                // Receitas do utilizador
                receipts = _context.Receitas
                    .Where(r => r.UtenteId == user.Id)
                    .ToList();
            }
            else if (userRoles.Contains("Doutor"))
            {
                // Receitas emitidas pelo médico
                receipts = _context.Receitas
                    .Where(r => r.DoutorId == user.Id)
                    .ToList();
            }
            else if (userRoles.Contains("Farmaceuta"))
            {
                // Receitas aviadas pelo farmacêutico
                receipts = _context.Receitas
                    .Where(r => r.FarmaceutaId == user.Id)
                    .ToList();
            }

            _logger.LogInformation("Utilizador {NumeroUtente} viu a lista de receitas", user.NumeroUtente);
            return View(receipts);
        }

        /// <summary>
        /// Ver todos os medicamentos disponíveis
        /// </summary>
        public async Task<IActionResult> ViewMedicamentos()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var medications = _context.Medicamentos.ToList();

            _logger.LogInformation("Utilizador {NumeroUtente} viu a lista de medicamentos", user.NumeroUtente);
            return View(medications);
        }

        /// <summary>
        /// Ver lista de utentes (apenas para médicos e farmacêuticos)
        /// </summary>
        [Authorize(Roles = "Doutor,Farmaceuta")]
        public async Task<IActionResult> ViewUtentes()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var utentes = _context.Utentes.ToList();

            _logger.LogInformation("Utilizador {NumeroUtente} viu a lista de utentes", user.NumeroUtente);
            return View(utentes);
        }

        /// <summary>
        /// Editar perfil do utilizador
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EditPerfil()
        {
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
        public async Task<IActionResult> EditPerfil(Utente model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            if (user.Id != model.Id)
                return Forbid();

            user.Nome = model.Nome;
            user.Email = model.Email;
            user.DataNascimento = model.DataNascimento;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("Utilizador {NumeroUtente} atualizou o perfil", user.NumeroUtente);
                return RedirectToAction("Index", "Dashboard");
            }

            return View(user);
        }

        /// <summary>
        /// Página para alterar palavra-passe
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
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
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                _logger.LogInformation("Utilizador {NumeroUtente} alterou a palavra-passe", user.NumeroUtente);
                return RedirectToAction("Index", "Dashboard");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }
    }

    public class ChangePasswordModel
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
