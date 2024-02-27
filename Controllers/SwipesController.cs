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
        private readonly ILogger<SwipesController> _logger;

        public SwipesController(TenderContext context, IUserService userService, ILogger<SwipesController> logger)
        {
            _context = context;
            _userService = userService;
            _logger = logger;
        }

        // GET: api/Swipes/5
        [HttpGet("{receiverId}")]
        public async Task<ActionResult<IEnumerable<Swipe>>> GetSwipes(string receiverId)
        {
            _logger.LogInformation("Fetching swipes for receiver ID {ReceiverId}", receiverId);

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
        public async Task<ActionResult<Swipe>> PostSwipe(string receiverId)
        {
            _logger.LogInformation("Attempting to create a swipe from {SenderId} to {ReceiverId}", _userService.CurrentUser.Id, receiverId);

            var receiver = await _context.Users.FindAsync(receiverId);
            if (receiver == null)
            {
                _logger.LogWarning("Attempted to swipe non-existent receiver ID {ReceiverId}", receiverId);
                return BadRequest("Receiver does not exist.");
            }

            var swipe = new Swipe
            {
                SenderId = _userService.CurrentUser.Id,
                ReceiverId = receiverId,
            };

            _context.Swipes.Add(swipe);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSwipes), new { receiverId = swipe.ReceiverId }, swipe);
        }

        // DELETE: api/Swipes/5
        [HttpDelete("{senderId}")]
        public async Task<IActionResult> DeleteSwipe(string senderId)
        {
            _logger.LogInformation("Deleting swipe from {SenderId} to current user {ReceiverId}", senderId, _userService.CurrentUser.Id);

            var swipe = await _context.Swipes
                .FirstOrDefaultAsync(s => s.SenderId == senderId && s.ReceiverId == _userService.CurrentUser.Id);

            if (swipe == null)
            {
                _logger.LogWarning("Swipe not found for deletion between {SenderId} and {ReceiverId}", senderId, _userService.CurrentUser.Id);
                return NotFound();
            }

            _context.Swipes.Remove(swipe);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
