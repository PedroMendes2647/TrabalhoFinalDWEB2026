using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.WebApp.Data;
using TrabalhoFinalDWEB2026.WebApp.Hubs;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.Doutor {
    [Authorize(Roles = "Doutor")]
    public class CreateReceitaModel : PageModel {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<CreateReceitaModel> _logger;
        private readonly IHubContext<ReceitaHub> _hubContext;

        public Utente? TargetUtente { get; set; }
        public List<Medicamentos>? Medicamentos { get; set; }

        [BindProperty]
        public List<int> MedicamentoIds { get; set; } = new();

        [BindProperty]
        public List<int> Quantidades { get; set; } = new();

        public CreateReceitaModel(
            ApplicationDbContext context,
            UserManager<Utente> userManager,
            ILogger<CreateReceitaModel> logger,
            IHubContext<ReceitaHub> hubContext) {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> OnGetAsync(string utenteId) {
            var currentDoctor = await _userManager.GetUserAsync(User);
            TargetUtente = await _userManager.FindByIdAsync(utenteId);

            if (TargetUtente == null) {
                return NotFound("Utilizador de destino não encontrado.");
            }

            if (currentDoctor != null && currentDoctor.Id == utenteId) {
                ModelState.AddModelError(string.Empty, "Não pode criar receita para si próprio.");
                return RedirectToPage("SearchUtente");
            }

            Medicamentos = await _context.Medicamentos.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string utenteId) {
            var currentDoctor = await _userManager.GetUserAsync(User);
            if (currentDoctor == null) {
                return RedirectToPage("/Account/Login");
            }

            TargetUtente = await _userManager.FindByIdAsync(utenteId);
            if (TargetUtente == null) {
                return NotFound("Utilizador não encontrado.");
            }

            if (currentDoctor.Id == utenteId) {
                return Forbid();
            }

            if (!ModelState.IsValid) {
                Medicamentos = await _context.Medicamentos.ToListAsync();
                return Page();
            }

            try {
                var receita = new Receita {
                    UtenteId = utenteId,
                    DoutorId = currentDoctor.Id,
                    DataEmissao = DateTime.Now,
                    Estado = Receita.State.Emitida
                };

                _context.Receitas.Add(receita);
                await _context.SaveChangesAsync();

                int totalMedicamentos = 0;
                if (MedicamentoIds != null && MedicamentoIds.Count > 0) {
                    for (int i = 0; i < MedicamentoIds.Count; i++) {
                        var medicamentoId = MedicamentoIds[i];
                        var quantidade = (i < Quantidades.Count) ? Quantidades[i] : 1;

                        var medicamento = await _context.Medicamentos.FindAsync(medicamentoId);
                        if (medicamento != null) {
                            var receitaMedicamento = new ReceitaMedicamentos {
                                ReceitaId = receita.Id,
                                MedicamentoId = medicamentoId,
                                Quantidade = quantidade,
                                Posologia = "Conforme indicação clínica"
                            };
                            _context.ReceitaMedicamentos.Add(receitaMedicamento);
                            totalMedicamentos++;
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("Médico {DoutorId} criou receita {ReceitaId} para utilizador {UtenteId}",
                    currentDoctor.Id, receita.Id, utenteId);

                // Toast de notificação
                await _hubContext.Clients.All.SendAsync(
                    "NotificacaoReceita",
                    $"O Dr. {currentDoctor.Nome} prescreveu uma nova receita para {TargetUtente.Nome}.",
                    "Nova Receita Emitida"
                );

                // Envio de dados estruturados para inserção direta na tabela em tempo real
                await _hubContext.Clients.All.SendAsync("ReceitaAdicionada", new {
                    id = receita.Id,
                    utenteId = targetUtenteId(utenteId),
                    utenteNome = TargetUtente.Nome,
                    doutorNome = currentDoctor.Nome,
                    dataEmissao = receita.DataEmissao.ToString("dd/MM/yyyy HH:mm"),
                    estado = "Emitida",
                    totalMedicamentos = totalMedicamentos
                });

                return RedirectToPage("ViewUtenteData", new { utenteId });
            } catch (Exception ex) {
                _logger.LogError(ex, "Erro ao criar receita para utente {UtenteId}", utenteId);
                ModelState.AddModelError(string.Empty, "Erro ao criar receita. Tente novamente.");
                Medicamentos = await _context.Medicamentos.ToListAsync();
                return Page();
            }
        }

        private string targetUtenteId(string id) => id;
    }
}