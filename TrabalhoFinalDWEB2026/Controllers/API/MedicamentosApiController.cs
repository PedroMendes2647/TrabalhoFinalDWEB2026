using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.Data;
using TrabalhoFinalDWEB2026.Models;

namespace TrabalhoFinalDWEB2026.Controllers.Api {
    [ApiController]
    [Route("api/[controller]")]
    public class MedicamentosApiController : ControllerBase {
        private readonly ApplicationDbContext _context;

        public MedicamentosApiController(ApplicationDbContext context) {
            _context = context;
        }



        /// <summary>
        ///  Obtém uma lista de medicamentos 
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Medicamentos>>> GetMedicamentos() {
            return await _context.Medicamentos.ToListAsync();
        }

        /// <summary>
        /// Obtém um medicamento específico pelo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Medicamentos>> GetMedicamento(int id) {
            var medicamento = await _context.Medicamentos.FindAsync(id);
            if (medicamento == null) return NotFound(new { mensagem = "Medicamento não encontrado." });
            return medicamento;
        }

        /// <summary>
        /// Cria um novo medicamento
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Medicamentos>> PostMedicamento(Medicamentos medicamento) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            _context.Medicamentos.Add(medicamento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMedicamento), new { id = medicamento.Id }, medicamento);
        }

        /// <summary>
        /// Atualiza os dados de um medicamento existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMedicamento(int id, Medicamentos medicamento) {
            if (id != medicamento.Id) {
                return BadRequest(new { mensagem = "O ID do URL não coincide com o ID do corpo." });
            }

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            _context.Entry(medicamento).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!_context.Medicamentos.Any(e => e.Id == id)) {
                    return NotFound(new { mensagem = "Medicamento não encontrado para atualizar." });
                }
                throw;
            }

            return Ok(new { mensagem = "Medicamento atualizado com sucesso!" });
        }

        /// <summary>
        /// Elimina um medicamento. Não permite eliminar medicamentos associados a receitas ativas.
        /// Regra de Negócio: Protección de referencial integrity
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicamento(int id) {
            var medicamento = await _context.Medicamentos.FindAsync(id);
            if (medicamento == null) {
                return NotFound(new { mensagem = "Medicamento não encontrado." });
            }

            // Verifica se o medicamento está associado a alguma receita
            var associadoReceita = await _context.ReceitaMedicamentos.AnyAsync(rm => rm.MedicamentoId == id);
            if (associadoReceita) {
                return BadRequest(new { mensagem = "Não é possível eliminar este medicamento porque ele está associado a receitas existentes." });
            }

            _context.Medicamentos.Remove(medicamento);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Medicamento eliminado com sucesso!" });
        }
    }
}
