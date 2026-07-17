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

        /// <summary>
        /// Painel de controlo acessível a todos os utilizadores autenticados
        /// Apresenta informações do utilizador atual
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound("Utilizador não encontrado.");
            }

            return View(currentUser);
        }

        /// <summary>
        /// Permite visualizar os dados pessoais do utilizador
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> MyData()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound("Utilizador não encontrado.");
            }

            return View(currentUser);
        }

        /// <summary>
        /// Lista todas as receitas do utilizador atual com medicamentos associados
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> MyReceitas()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound("Utilizador não encontrado.");
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

        /// <summary>
        /// Visualiza os detalhes de uma receita específica
        /// Apenas permite visualizar receitas do próprio utilizador ou se for Doutor/Farmaceuta
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ReceitaDetails(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound("Utilizador não encontrado.");
            }

            var receita = await _context.Receitas
                .Include(r => r.ListaDeMedicamentos)
                .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.Doutor)
                .Include(r => r.Farmaceuta)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receita == null)
            {
                return NotFound("Receita não encontrada.");
            }

            // Restrição: Apenas o utilizador, o doutor ou o farmaceuta podem ver a receita
            var userRoles = await _userManager.GetRolesAsync(currentUser);
            if (receita.UtenteId != currentUser.Id && 
                !userRoles.Contains("Doutor") && 
                !userRoles.Contains("Farmaceuta"))
            {
                return Forbid();
            }

            return View(receita);
        }

        /// <summary>
        /// Apresenta o formulário para editar os dados pessoais do utilizador
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EditData()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound("Utilizador não encontrado.");
            }

            return View(currentUser);
        }

        /// <summary>
        /// Processa a atualização dos dados pessoais do utilizador
        /// Permite atualizar Nome, Data de Nascimento e Email
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditData(Utente model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound("Utilizador não encontrado.");
            }

            if (ModelState.IsValid)
            {
                currentUser.Nome = model.Nome;
                currentUser.DataNascimento = model.DataNascimento;
                currentUser.Email = model.Email;

                var result = await _userManager.UpdateAsync(currentUser);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Utilizador atualizou seus dados. Número: {NumeroUtente}", currentUser.NumeroUtente);
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
