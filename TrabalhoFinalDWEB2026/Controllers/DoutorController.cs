using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.Data;
using TrabalhoFinalDWEB2026.Models;

namespace TrabalhoFinalDWEB2026.Controllers
{
    [Authorize(Roles = "Doutor")]
    public class DoutorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<DoutorController> _logger;

        public DoutorController(
            ApplicationDbContext context,
            UserManager<Utente> userManager,
            ILogger<DoutorController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // Doctor dashboard
        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

        // Search for a utente by NumeroUtente
        [HttpGet]
        public IActionResult SearchUtente()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchUtente(string numeroUtente)
        {
            if (string.IsNullOrWhiteSpace(numeroUtente))
            {
                ModelState.AddModelError("numeroUtente", "Please enter a Numero Utente.");
                return View();
            }

            var utente = await _userManager.FindByNameAsync(numeroUtente);
            if (utente == null)
            {
                ModelState.AddModelError("numeroUtente", "Utente not found.");
                return View();
            }

            // Redirect to view that utente's details
            return RedirectToAction("ViewUtenteData", new { utenteId = utente.Id });
        }

        // View another utente's data and receitas
        [HttpGet]
        public async Task<IActionResult> ViewUtenteData(string utenteId)
        {
            var utente = await _userManager.FindByIdAsync(utenteId);
            if (utente == null)
            {
                return NotFound("Utente not found.");
            }

            var receitas = await _context.Receitas
                .Where(r => r.UtenteId == utenteId)
                .Include(r => r.ListaDeMedicamentos)
                .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.Doutor)
                .Include(r => r.Farmaceuta)
                .OrderByDescending(r => r.DataEmissao)
                .ToListAsync();

            ViewBag.Utente = utente;
            ViewBag.Receitas = receitas;

            return View(utente);
        }

        // Create a new receita for another utente
        [HttpGet]
        public async Task<IActionResult> CreateReceita(string utenteId)
        {
            var currentDoctor = await _userManager.GetUserAsync(User);
            var targetUtente = await _userManager.FindByIdAsync(utenteId);

            if (targetUtente == null)
            {
                return NotFound("Target Utente not found.");
            }

            // Prevent doctor from creating receita for themselves
            if (currentDoctor.Id == utenteId)
            {
                return Forbid("Cannot create receita for yourself.");
            }

            var medicamentos = await _context.Medicamentos.ToListAsync();

            ViewBag.UtenteId = utenteId;
            ViewBag.UtenteName = targetUtente.Nome;
            ViewBag.Medicamentos = medicamentos;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReceita(string utenteId, CreateReceitaModel model)
        {
            var currentDoctor = await _userManager.GetUserAsync(User);
            var currentDoutorEntity = await _context.Doutores.FirstOrDefaultAsync(d => d.Id == currentDoctor.Id);

            if (currentDoutorEntity == null)
            {
                _logger.LogWarning("User with role Doutor is not a Doutor entity. UserId: {UserId}", currentDoctor.Id);
                return Forbid("You must be registered as a Doutor to create receitas.");
            }

            var targetUtente = await _userManager.FindByIdAsync(utenteId);
            if (targetUtente == null)
            {
                return NotFound("Target Utente not found.");
            }

            // Prevent doctor from creating receita for themselves
            if (currentDoctor.Id == utenteId)
            {
                return Forbid("Cannot create receita for yourself.");
            }

            if (ModelState.IsValid)
            {
                var receita = new Receita
                {
                    UtenteId = utenteId,
                    DoutorId = currentDoctor.Id,
                    DataEmissao = DateTime.Now,
                    Estado = "Emitida"
                };

                _context.Receitas.Add(receita);
                await _context.SaveChangesAsync();

                // Add medicamentos to the receita
                if (model.MedicamentoIds != null && model.MedicamentoIds.Any())
                {
                    foreach (var medicamentoId in model.MedicamentoIds)
                    {
                        var medicamento = await _context.Medicamentos.FindAsync(medicamentoId);
                        if (medicamento != null)
                        {
                            var receitaMedicamento = new ReceitaMedicamentos
                            {
                                ReceitaId = receita.Id,
                                MedicamentoId = medicamentoId,
                                Quantidade = model.Quantidade
                            };
                            _context.ReceitaMedicamentos.Add(receitaMedicamento);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("Doutor {DoutorId} created receita {ReceitaId} for Utente {UtenteId}", 
                    currentDoctor.Id, receita.Id, utenteId);

                return RedirectToAction("ViewUtenteData", new { utenteId });
            }

            var medicamentos = await _context.Medicamentos.ToListAsync();
            ViewBag.UtenteId = utenteId;
            ViewBag.Medicamentos = medicamentos;

            return View(model);
        }
    }

    public class CreateReceitaModel
    {
        public List<int> MedicamentoIds { get; set; } = new List<int>();
        public int Quantidade { get; set; } = 1;
    }
}
