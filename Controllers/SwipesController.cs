using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WADProject1.Models;
using WADProject1.Services;

namespace WADProject1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SwipesController : ControllerBase
    {
        private readonly TenderContext _context;
        private readonly IUserService _userService;

        public SwipesController(TenderContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        // GET: api/Swipes/5
        [HttpGet("{receiverId}")]
        public async Task<ActionResult<IEnumerable<Swipe>>> GetSwipes(int receiverId)
        {
            var swipes = await _context.Swipes
                .AsNoTracking()
                .Include(s => s.Sender)
                .Include(s => s.Receiver)
                .Where(s => s.ReceiverId == receiverId)
                .ToListAsync();

            return Ok(swipes);
        }

        // POST: api/Swipes/5
        [HttpPost("{receiverId}")]
        public async Task<ActionResult<Swipe>> PostSwipe(int receiverId)
        {
            var sender = _userService.CurrentUser;
            var receiver = await _context.Users.FindAsync(receiverId);

            // Check if the receiver exists
            if (receiver == null)
            {
                return BadRequest("Receiver does not exist.");
            }

            var swipe = new Swipe
            {
                SenderId = sender.UserId,
                ReceiverId = receiverId,
            };

            _context.Swipes.Add(swipe);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSwipes), new { receiverId = swipe.ReceiverId }, swipe);
        }

        // DELETE: api/Swipes/5
        [HttpDelete("{senderId}")]
        public async Task<IActionResult> DeleteSwipe(int senderId)
        {
            var receiverId = _userService.CurrentUser.UserId;

            var swipe = await _context.Swipes
                .FirstOrDefaultAsync(s => s.SenderId == senderId && s.ReceiverId == receiverId);

            if (swipe == null)
            {
                return NotFound();
            }

            _context.Swipes.Remove(swipe);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
