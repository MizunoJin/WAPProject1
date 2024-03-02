using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WADProject1.Models;

namespace WADProject1.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly TenderContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(TenderContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            _logger.LogInformation("Getting user with ID: {UserID}", id);

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                _logger.LogWarning("GetUser({UserID}) NOT FOUND", id);
                return NotFound();
            }

            return user;
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            _logger.LogInformation("Deleting user with ID: {UserID}", id);

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogWarning("DeleteUser({UserID}) NOT FOUND", id);
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User with ID: {UserID} deleted successfully", id);

            return NoContent();
        }
    }
}
