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

        /// <summary>
        /// Painel de controlo do médico
        /// </summary>
        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

        /// <summary>
        /// Apresenta o formulário para pesquisar um utilizador pelo NumeroUtente
        /// </summary>
        [HttpGet]
        public IActionResult SearchUtente()
        {
            return View();
        }

        /// <summary>
        /// Processa a pesquisa de um utilizador pelo NumeroUtente
        /// Redireciona para a página de detalhes do utilizador encontrado
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchUtente(string numeroUtente)
        {
            if (string.IsNullOrWhiteSpace(numeroUtente))
            {
                ModelState.AddModelError("numeroUtente", "Por favor, introduza um Número de Utente.");
                return View();
            }

            var utente = await _userManager.FindByNameAsync(numeroUtente);
            if (utente == null)
            {
                ModelState.AddModelError("numeroUtente", "Utilizador não encontrado.");
                return View();
            }

            // Redireciona para a página de detalhes do utilizador
            return RedirectToAction("ViewUtenteData", new { utenteId = utente.Id });
        }

        /// <summary>
        /// Visualiza os dados e receitas de um utilizador específico
        /// Apenas acessível a médicos
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ViewUtenteData(string utenteId)
        {
            var utente = await _userManager.FindByIdAsync(utenteId);
            if (utente == null)
            {
                return NotFound("Utilizador não encontrado.");
            }

            var receitas = await _context.Receitas
                .Where(r => r.UtenteId == utenteId)
                .Include(r => r.Utente)
                .Include(r => r.ListaDeMedicamentos)
                .ThenInclude(rm => rm.Medicamento)
                .Include(r => r.DoutorUtente)
                .Include(r => r.FarmaceutaUtente)
                .OrderByDescending(r => r.DataEmissao)
                .ToListAsync();

            ViewBag.Utente = utente;
            ViewBag.Receitas = receitas;

            return View(utente);
        }

        /// <summary>
        /// Apresenta o formulário para criar uma nova receita para um utilizador
        /// Restrição: Um médico não pode criar receita para si próprio
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreateReceita(string utenteId)
        {
            var currentDoctor = await _userManager.GetUserAsync(User);
            var targetUtente = await _userManager.FindByIdAsync(utenteId);

            if (targetUtente == null)
            {
                return NotFound("Utilizador de destino não encontrado.");
            }

            // Impede que o médico crie receita para si próprio
            if (currentDoctor.Id == utenteId)
            {
                ModelState.AddModelError(string.Empty, "Não pode criar receita para si próprio.");
                return RedirectToAction("SearchUtente");
            }

            var medicamentos = await _context.Medicamentos.ToListAsync();

            ViewBag.UtenteId = utenteId;
            ViewBag.UtenteName = targetUtente.Nome;
            ViewBag.Medicamentos = medicamentos;

            return View();
        }

        /// <summary>
        /// Processa a criação de uma nova receita para um utilizador
        /// Adiciona os medicamentos à receita através da relação M:N
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReceita(string utenteId, CreateReceitaModel model)
        {
            try
            {
                var currentDoctor = await _userManager.GetUserAsync(User);
                if (currentDoctor == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var targetUtente = await _userManager.FindByIdAsync(utenteId);
                if (targetUtente == null)
                {
                    return NotFound("Utilizador de destino não encontrado.");
                }

                // Impede que o médico crie receita para si próprio
                if (currentDoctor.Id == utenteId)
                {
                    ModelState.AddModelError("", "Não pode criar receita para si próprio.");
                    var medicamentosError = await _context.Medicamentos.ToListAsync();
                    ViewBag.UtenteId = utenteId;
                    ViewBag.UtenteName = targetUtente.Nome;
                    ViewBag.Medicamentos = medicamentosError;
                    return View(model);
                }

                if (ModelState.IsValid)
                {
                    // Validar se tem pelo menos um medicamento
                    if (model.MedicamentoIds == null || !model.MedicamentoIds.Any())
                    {
                        ModelState.AddModelError("", "Deve selecionar pelo menos um medicamento.");
                        var medicamentos = await _context.Medicamentos.ToListAsync();
                        ViewBag.UtenteId = utenteId;
                        ViewBag.Medicamentos = medicamentos;
                        return View(model);
                    }

                    var receita = new Receita
                    {
                        UtenteId = utenteId,
                        DoutorId = currentDoctor.Id,
                        DataEmissao = DateTime.Now,
                        Estado = "Emitida"
                    };

                    _context.Receitas.Add(receita);
                    await _context.SaveChangesAsync();

                    // Adiciona os medicamentos à receita
                    if (model.MedicamentoIds != null && model.MedicamentoIds.Any())
                    {
                        for (int i = 0; i < model.MedicamentoIds.Count; i++)
                        {
                            var medicamentoId = model.MedicamentoIds[i];
                            var quantidade = (i < model.Quantidades.Count) ? model.Quantidades[i] : 1;

                            var medicamento = await _context.Medicamentos.FindAsync(medicamentoId);
                            if (medicamento != null)
                            {
                                var receitaMedicamento = new ReceitaMedicamentos
                                {
                                    ReceitaId = receita.Id,
                                    MedicamentoId = medicamentoId,
                                    Quantidade = quantidade
                                };
                                _context.ReceitaMedicamentos.Add(receitaMedicamento);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                    _logger.LogInformation("Médico {DoutorId} criou receita {ReceitaId} para utilizador {UtenteId}", 
                        currentDoctor.Id, receita.Id, utenteId);

                    return RedirectToAction("ViewUtenteData", new { utenteId });
                }

                var medicamentosReturn = await _context.Medicamentos.ToListAsync();
                ViewBag.UtenteId = utenteId;
                ViewBag.Medicamentos = medicamentosReturn;

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar receita para utilizador {UtenteId}", utenteId);
                ModelState.AddModelError("", "Ocorreu um erro ao processar o pedido. Por favor, tente novamente.");

                var medicamentos = await _context.Medicamentos.ToListAsync();
                ViewBag.UtenteId = utenteId;
                ViewBag.Medicamentos = medicamentos;
                return View(model);
            }
        }
    }

    public class CreateReceitaModel
    {
        public List<int> MedicamentoIds { get; set; } = new List<int>();
        public List<int> Quantidades { get; set; } = new List<int>();
    }
}
