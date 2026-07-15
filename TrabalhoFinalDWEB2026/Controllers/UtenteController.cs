using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.Data;
using TrabalhoFinalDWEB2026.Models;

namespace TrabalhoFinalDWEB2026.Controllers
{
    [Authorize]
    public class UtenteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<UtenteController> _logger;

        public UtenteController(
            ApplicationDbContext context,
            UserManager<Utente> userManager,
            ILogger<UtenteController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // Dashboard accessible to all authenticated users
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound("User not found.");
            }

            return View(currentUser);
        }

        // View personal data
        [HttpGet]
        public async Task<IActionResult> MyData()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound("User not found.");
            }

            return View(currentUser);
        }

        // View personal receitas
        [HttpGet]
        public async Task<IActionResult> MyReceitas()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound("User not found.");
            }

            var receitas = await _context.Receitas
                .Where(r => r.UtenteId == currentUser.Id)
                .Include(r => r.ListaDeMedicamentos)
                .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.Doutor)
                .Include(r => r.Farmaceuta)
                .OrderByDescending(r => r.DataEmissao)
                .ToListAsync();

            return View(receitas);
        }

        // View receita details
        [HttpGet]
        public async Task<IActionResult> ReceitaDetails(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound("User not found.");
            }

            var receita = await _context.Receitas
                .Include(r => r.ListaDeMedicamentos)
                .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.Doutor)
                .Include(r => r.Farmaceuta)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receita == null)
            {
                return NotFound("Receita not found.");
            }

            // Only allow viewing own receitas or if doctor/pharmacist assigned
            var userRoles = await _userManager.GetRolesAsync(currentUser);
            if (receita.UtenteId != currentUser.Id && 
                !userRoles.Contains("Doutor") && 
                !userRoles.Contains("Farmaceuta"))
            {
                return Forbid();
            }

            return View(receita);
        }

        // Edit personal data
        [HttpGet]
        public async Task<IActionResult> EditData()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound("User not found.");
            }

            return View(currentUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditData(Utente model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound("User not found.");
            }

            if (ModelState.IsValid)
            {
                currentUser.Nome = model.Nome;
                currentUser.DataNascimento = model.DataNascimento;
                currentUser.Email = model.Email;

                var result = await _userManager.UpdateAsync(currentUser);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User updated their data. Numero Utente: {NumeroUtente}", currentUser.NumeroUtente);
                    return RedirectToAction("MyData");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
    }
}
