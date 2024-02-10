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

        public MatchesController(TenderContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        // GET: api/Matches/userId - Retrieves matches where the user is either sender or receiver
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Match>>> GetMatches(int userId)
        {
            var matches = await _context.Matches
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .ToListAsync();

            return matches;
        }

        // POST: api/Matches/receiverId - Creates a match with the logged-in user as the sender
        [HttpPost("{receiverId}")]
        public async Task<ActionResult<Match>> PostMatch(int receiverId)
        {
            var senderId = _userService.CurrentUser.UserId; // Assuming IUserService is correctly implemented to get the current user

            // Ensure the receiver exists
            var receiverExists = await _context.Users.AnyAsync(u => u.UserId == receiverId);
            if (!receiverExists)
            {
                return BadRequest("Receiver does not exist.");
            }

            var match = new Match
            {
                SenderId = senderId,
                ReceiverId = receiverId
            };

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMatch", new { id = match.MatchId }, match);
        }

        // DELETE: api/Matches/userId - Deletes a match where the current user is involved as either sender or receiver
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteMatch(int userId)
        {
            var currentUserId = _userService.CurrentUser.UserId; // Assuming IUserService is correctly implemented

            // Find match where the current user is either sender or receiver
            var match = await _context.Matches
                .FirstOrDefaultAsync(m => (m.SenderId == currentUserId && m.ReceiverId == userId) || 
                                          (m.ReceiverId == currentUserId && m.SenderId == userId));
            if (match == null)
            {
                return NotFound();
            }

            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
