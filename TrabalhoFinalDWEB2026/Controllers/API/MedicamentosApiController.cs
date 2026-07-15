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


        //GET: api/MedicamentosApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Medicamentos>>> GetMedicamentos() {
            return await _context.Medicamentos.ToListAsync();
        }

        // GET: api/MedicamentosApi/X
        [HttpGet("{id}")]
        public async Task<ActionResult<Medicamentos>> GetMedicamento(int id) {
            var medicamento = await _context.Medicamentos.FindAsync(id);
            if (medicamento == null) return NotFound(new { mensagem = "Medicamento não encontrado." });
            return medicamento;
        }

        // POST: api/MedicamentosApi
        [HttpPost]
        public async Task<ActionResult<Medicamentos>> PostMedicamento(Medicamentos medicamento) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            _context.Medicamentos.Add(medicamento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMedicamento), new { id = medicamento.Id }, medicamento);
        }

        // PUT: api/MedicamentosApi/5
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

        
        //DELETE: api/MedicamentosApi/
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicamento(int id) {
            var medicamento = await _context.Medicamentos.FindAsync(id);
            if (medicamento == null) {
                return NotFound(new { mensagem = "Medicamento não encontrado." });
            }

            // Regra de Negócio: Não deixar eliminar um medicamento que esteja associado a alguma receita ativa
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
