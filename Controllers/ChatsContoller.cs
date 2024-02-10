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
    public class ChatsController : ControllerBase
    {
        private readonly TenderContext _context;
        private readonly IUserService _userService;

        public ChatsController(TenderContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        // GET: api/Chats/userId - Retrieves chats where the user is either sender or receiver
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Chat>>> GetChats(int userId)
        {
            var currentUserId = _userService.CurrentUser.UserId; // Assuming IUserService is implemented

            var chats = await _context.Chats
                .Include(c => c.Sender)
                .Include(c => c.Receiver)
                .Where(c => (c.SenderId == currentUserId && c.ReceiverId == userId) || 
                            (c.ReceiverId == currentUserId && c.SenderId == userId))
                .ToListAsync();

            return chats;
        }

        // POST: api/Chats/receiverId - Creates a chat message with the logged-in user as the sender
        [HttpPost("{receiverId}")]
        public async Task<ActionResult<Chat>> PostChat(int receiverId, Chat chat)
        {
            var senderId = _userService.CurrentUser.UserId; // Assuming IUserService is implemented

            // Prevent from setting different senderId in the body
            if (chat.SenderId != senderId)
            {
                return BadRequest("You can only send messages as yourself.");
            }

            chat.SenderId = senderId; // Ensure the senderId is the current user
            chat.ReceiverId = receiverId;

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChats", new { userId = receiverId }, chat);
        }

        // DELETE: api/Chats/userId - Deletes a chat message involving the current user and the specified userId
        [HttpDelete("{chatId}")]
        public async Task<IActionResult> DeleteChat(int chatId)
        {
            var currentUserId = _userService.CurrentUser.UserId; // Assuming IUserService is implemented

            var chat = await _context.Chats
                .FirstOrDefaultAsync(c => c.ChatId == chatId && 
                                          (c.SenderId == currentUserId || c.ReceiverId == currentUserId));
            if (chat == null)
            {
                return NotFound();
            }

            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
