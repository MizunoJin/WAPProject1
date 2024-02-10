using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WADProject1.Models;

namespace WADProject1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SwipesController : ControllerBase
    {
        private readonly TenderContext _context;

        public SwipesController(TenderContext context)
        {
            _context = context;
        }

        // GET: api/Swipes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Swipe>>> GetSwipes()
        {
            return await _context.Swipes
                .Include(s => s.Sender)
                .Include(s => s.Receiver)
                .ToListAsync();
        }

        // GET: api/Swipes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Swipe>> GetSwipe(int id)
        {
            var swipe = await _context.Swipes
                .Include(s => s.Sender)
                .Include(s => s.Receiver)
                .FirstOrDefaultAsync(s => s.SwipeId == id);

            if (swipe == null)
            {
                return NotFound();
            }

            return swipe;
        }

        // POST: api/Swipes
        [HttpPost]
        public async Task<ActionResult<Swipe>> PostSwipe(Swipe swipe)
        {
            _context.Swipes.Add(swipe);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSwipe), new { id = swipe.SwipeId }, swipe);
        }

        // PUT: api/Swipes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSwipe(int id, Swipe swipe)
        {
            if (id != swipe.SwipeId)
            {
                return BadRequest();
            }

            _context.Entry(swipe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SwipeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Swipes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSwipe(int id)
        {
            var swipe = await _context.Swipes.FindAsync(id);
            if (swipe == null)
            {
                return NotFound();
            }

            _context.Swipes.Remove(swipe);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SwipeExists(int id)
        {
            return _context.Swipes.Any(e => e.SwipeId == id);
        }
    }
}
