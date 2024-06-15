using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReaccionController : ControllerBase
    {
        private readonly IReaccionService _reaccionService;

        public ReaccionController(IReaccionService reaccionService)
        {
            _reaccionService = reaccionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reaccion>>> GetReacciones()
        {
            var reacciones = await _reaccionService.GetAsync();
            return Ok(reacciones);
        }

        [HttpGet("{id:length(24)}", Name = "GetReaccion")]
        public async Task<ActionResult<Reaccion>> GetReaccion(string id)
        {
            var reaccion = await _reaccionService.GetAsync(id);

            if (reaccion == null)
            {
                return NotFound();
            }

            return Ok(reaccion);
        }

        [HttpPost]
        public async Task<ActionResult<Reaccion>> CreateReaccion(Reaccion reaccion)
        {
            await _reaccionService.CreateAsync(reaccion);
            return CreatedAtRoute("GetReaccion", new { id = reaccion.Id }, reaccion);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> UpdateReaccion(string id, Reaccion updatedReaccion)
        {
            var reaccion = await _reaccionService.GetAsync(id);

            if (reaccion == null)
            {
                return NotFound();
            }

            updatedReaccion.Id = reaccion.Id;
            await _reaccionService.UpdateAsync(id, updatedReaccion);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeleteReaccion(string id)
        {
            var reaccion = await _reaccionService.GetAsync(id);

            if (reaccion == null)
            {
                return NotFound();
            }

            await _reaccionService.RemoveAsync(id);
            return NoContent();
        }
    }
}
