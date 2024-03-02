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
        private readonly ILogger<ChatsController> _logger;

        public ChatsController(TenderContext context, IUserService userService, ILogger<ChatsController> logger)
        {
            _context = context;
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Chat>>> GetChats()
        {
            _logger.LogInformation("Retrieving all chats");

            var currentUserId = _userService.CurrentUser.Id;

            var chats = await _context.Chats
                .Include(c => c.Sender)
                .Include(c => c.Receiver)
                .Where(c => c.SenderId == currentUserId || c.ReceiverId == currentUserId)
                .ToListAsync();

            return chats;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Chat>>> GetChats(string userId)
        {
            _logger.LogInformation("Retrieving chats for user ID {UserId}", userId);

            var currentUserId = _userService.CurrentUser.Id;

            var chats = await _context.Chats
                .Include(c => c.Sender)
                .Include(c => c.Receiver)
                .Where(c => (c.SenderId == currentUserId && c.ReceiverId == userId) ||
                            (c.ReceiverId == currentUserId && c.SenderId == userId))
                .ToListAsync();

            return chats;
        }

        [HttpPost("{receiverId}")]
        public async Task<ActionResult<Chat>> PostChat(string receiverId, Chat chat)
        {
            _logger.LogInformation("Creating a chat message from {SenderId} to {ReceiverId}", _userService.CurrentUser.Id, receiverId);

            chat.SenderId = _userService.CurrentUser.Id;
            chat.ReceiverId = receiverId;

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChats", new { userId = receiverId }, chat);
        }

        [HttpDelete("{chatId}")]
        public async Task<IActionResult> DeleteChat(int chatId)
        {
            _logger.LogInformation("Deleting chat message with ID {ChatId}", chatId);

            var currentUserId = _userService.CurrentUser.Id;

            var chat = await _context.Chats
                .FirstOrDefaultAsync(c => c.ChatId == chatId &&
                                          (c.SenderId == currentUserId || c.ReceiverId == currentUserId));
            if (chat == null)
            {
                _logger.LogWarning("Chat message not found for deletion: {ChatId}", chatId);
                return NotFound();
            }

            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
