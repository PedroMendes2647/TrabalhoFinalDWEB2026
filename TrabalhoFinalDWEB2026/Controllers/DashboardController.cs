using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrabalhoFinalDWEB2026.Models;

namespace TrabalhoFinalDWEB2026.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            UserManager<Utente> userManager,
            ILogger<DashboardController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Redireciona para o painel apropriado com base na função do utilizador
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var roles = await _userManager.GetRolesAsync(user);
            _logger.LogInformation("Utilizador {NumeroUtente} tem funções: {Roles}", user.NumeroUtente, string.Join(", ", roles));

            // Verifica cada função e redireciona para o painel apropriado
            if (roles.Contains("Doutor"))
            {
                return RedirectToAction("DoutorBoard");
            }
            else if (roles.Contains("Farmaceuta"))
            {
                return RedirectToAction("FarmaceutaBoard");
            }
            else if (roles.Contains("Utente"))
            {
                return RedirectToAction("UtenteBoard");
            }

            // Fallback padrão
            return RedirectToAction("UtenteBoard");
        }

        /// <summary>
        /// Painel do Utente
        /// </summary>
        [Authorize(Roles = "Utente")]
        public async Task<IActionResult> UtenteBoard()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        /// <summary>
        /// Painel do Médico
        /// </summary>
        [Authorize(Roles = "Doutor")]
        public async Task<IActionResult> DoutorBoard()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        /// <summary>
        /// Painel do Farmaceuta
        /// </summary>
        [Authorize(Roles = "Farmaceuta")]
        public async Task<IActionResult> FarmaceutaBoard()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }
    }
}
