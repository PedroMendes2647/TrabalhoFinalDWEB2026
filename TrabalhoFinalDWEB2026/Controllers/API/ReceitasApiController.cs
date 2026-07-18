using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.Data;
using TrabalhoFinalDWEB2026.Models;

namespace TrabalhoFinalDWEB2026.Controllers.Api {
    [ApiController]
    [Route("api/[controller]")]
    public class ReceitasApiController : ControllerBase {
        private readonly ApplicationDbContext _context;

        public ReceitasApiController(ApplicationDbContext context) {
            _context = context;
        }

        /// <summary>
        /// Obtém a lista completa de todas as receitas com as relações associadas
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Receita>>> GetReceitas() {
            return await _context.Receitas
                .Include(r => r.Utente)
                .Include(r => r.DoutorUtente)
                .Include(r => r.FarmaceutaUtente)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém uma receita específica pelo ID com todos os seus medicamentos associados
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Receita>> GetReceita(int id) {
            var receita = await _context.Receitas
                .Include(r => r.Utente)
                .Include(r => r.DoutorUtente)
                .Include(r => r.FarmaceutaUtente)
                .Include(r => r.ListaDeMedicamentos)
                    .ThenInclude(rm => rm.Medicamento)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receita == null) return NotFound(new { mensagem = "Receita não encontrada." });
            return receita;
        }

        /// <summary>
        /// Cria uma nova receita com medicamentos associados
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Receita>> PostReceita(ReceitaInputDto input) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var receita = new Receita {
                UtenteId = input.UtenteId,
                DoutorId = input.DoutorId,
                DataEmissao = DateTime.Now,
                Estado = "Emitida"
            };

            _context.Receitas.Add(receita);
            await _context.SaveChangesAsync();

            // Adiciona os medicamentos à receita
            if (input.Medicamentos != null) {
                foreach (var medInput in input.Medicamentos) {
                    var receitaMedicamento = new ReceitaMedicamentos {
                        ReceitaId = receita.Id,
                        MedicamentoId = medInput.MedicamentoId,
                        Quantidade = medInput.Quantidade,
                        Posologia = medInput.Posologia
                    };
                    _context.ReceitaMedicamentos.Add(receitaMedicamento);
                }
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetReceita), new { id = receita.Id }, receita);
        }

        /// <summary>
        /// Atualiza os dados de uma receita e seus medicamentos associados.
        /// Restrição: Não permite editar receitas já aviadas pelo farmacêutico.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReceita(int id, ReceitaInputDto input) {
            var receita = await _context.Receitas
                .Include(r => r.ListaDeMedicamentos)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receita == null) {
                return NotFound(new { mensagem = "Receita não encontrada para atualização." });
            }

            // Regra de Negócio: Não permitir editar receitas já aviadas pelo farmacêutico
            if (receita.Estado == "Aviada") {
                return BadRequest(new { mensagem = "Não é permitido alterar dados de uma receita já aviada." });
            }

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            // Atualiza os dados básicos
            receita.UtenteId = input.UtenteId;
            receita.DoutorId = input.DoutorId;

            // Remove os medicamentos antigos da relação M:N para reinserir os novos atualizados
            _context.ReceitaMedicamentos.RemoveRange(receita.ListaDeMedicamentos);

            // Adiciona a nova lista atualizada
            if (input.Medicamentos != null) {
                foreach (var medInput in input.Medicamentos) {
                    var receitaMedicamento = new ReceitaMedicamentos {
                        ReceitaId = receita.Id,
                        MedicamentoId = medInput.MedicamentoId,
                        Quantidade = medInput.Quantidade,
                        Posologia = medInput.Posologia
                    };
                    _context.ReceitaMedicamentos.Add(receitaMedicamento);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensagem = "Receita e medicamentos associados atualizados com sucesso!" });
        }

        /// <summary>
        /// Elimina uma receita.
        /// Restrição: Não permite eliminar receitas já entregues/aviadas pelo farmacêutico.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReceita(int id) {
            var receita = await _context.Receitas.FindAsync(id);
            if (receita == null) {
                return NotFound(new { mensagem = "Receita não encontrada." });
            }

            // Regra de Negócio: Impedir eliminação de receitas já entregues/aviadas
            if (receita.Estado == "Aviada") {
                return BadRequest(new { mensagem = "Não é permitido eliminar receitas que já tenham sido aviadas." });
            }

            _context.Receitas.Remove(receita);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Receita eliminada com sucesso!" });
        }
    }

    public class ReceitaInputDto {
        public string UtenteId { get; set; } = string.Empty;
        public string DoutorId { get; set; } = string.Empty;
        public List<MedicamentoInputDto> Medicamentos { get; set; } = new();
    }

    public class MedicamentoInputDto {
        public int MedicamentoId { get; set; }
        public int Quantidade { get; set; }
        public string Posologia { get; set; } = string.Empty;
    }
}
