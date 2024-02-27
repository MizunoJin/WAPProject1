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
    public class MatchesController : ControllerBase
    {
        private readonly TenderContext _context;
        private readonly IUserService _userService;
        private readonly ILogger<MatchesController> _logger;

        public MatchesController(TenderContext context, IUserService userService, ILogger<MatchesController> logger)
        {
            _context = context;
            _userService = userService;
            _logger = logger;
        }

        // GET: api/Matches/userId
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Match>>> GetMatches(string userId)
        {
            _logger.LogInformation("Retrieving matches for user ID {UserId}", userId);

            var matches = await _context.Matches
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .ToListAsync();

            return matches;
        }

        // POST: api/Matches/receiverId
        [HttpPost("{receiverId}")]
        public async Task<ActionResult<Match>> PostMatch(string receiverId)
        {
            _logger.LogInformation("Attempting to create a match with receiver ID {ReceiverId} by sender ID {SenderId}", receiverId, _userService.CurrentUser.Id);

            var receiverExists = await _context.Users.AnyAsync(u => u.Id == receiverId);
            if (!receiverExists)
            {
                _logger.LogWarning("Attempted to create a match with non-existent receiver ID {ReceiverId}", receiverId);
                return BadRequest("Receiver does not exist.");
            }

            var match = new Match
            {
                SenderId = _userService.CurrentUser.Id,
                ReceiverId = receiverId
            };

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMatch", new { id = match.MatchId }, match);
        }

        // DELETE: api/Matches/userId
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteMatch(string userId)
        {
            _logger.LogInformation("Attempting to delete a match involving user ID {UserId}", userId);

            var currentUserId = _userService.CurrentUser.Id;
            var match = await _context.Matches
                .FirstOrDefaultAsync(m => (m.SenderId == currentUserId && m.ReceiverId == userId) ||
                                          (m.ReceiverId == currentUserId && m.SenderId == userId));

            if (match == null)
            {
                _logger.LogWarning("No match found for deletion involving user ID {UserId} and current user ID {CurrentUserId}", userId, currentUserId);
                return NotFound();
            }

            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
