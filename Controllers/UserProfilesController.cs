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
    public class UserProfilesController : ControllerBase
    {
        private readonly TenderContext _context;
        private readonly IUserService _userService;
        private readonly ILogger<UserProfilesController> _logger;

        public UserProfilesController(TenderContext context, IUserService userService, ILogger<UserProfilesController> logger)
        {
            _context = context;
            _userService = userService;
            _logger = logger;
        }

        // GET: api/UserProfiles
        [HttpGet]
        public async Task<ActionResult<UserProfile>> GetUserProfile()
        {
            _logger.LogInformation("Getting user profile");

            var currentUser = _userService.CurrentUser;

            var currentUserId = _userService.CurrentUser.Id;

            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(up => up.UserId == currentUserId);

            if (userProfile == null)
            {
                _logger.LogWarning("User profile not found for user ID {UserId}", currentUserId);
                return NotFound();
            }

            return userProfile;
        }

        // PUT: api/UserProfiles
        [HttpPut]
        public async Task<IActionResult> PutUserProfile(UserProfile userProfile)
        {
            _logger.LogInformation("Updating user profile for user ID {UserId}", userProfile.UserId);

            var currentUser = _userService.CurrentUser;

            if (userProfile.UserId != currentUser.Id)
            {
                _logger.LogWarning("Attempt to update profile for a different user");
                return BadRequest("You can only update your own profile.");
            }

            var existingProfile = await _context.UserProfiles.FindAsync(userProfile.UserProfileId);
            if (existingProfile == null)
            {
                _logger.LogWarning("User profile {UserProfileId} not found", userProfile.UserProfileId);
                return NotFound();
            }

            _context.Entry(existingProfile).CurrentValues.SetValues(userProfile);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user profile");
                throw;
            }

            return NoContent();
        }

        // POST: api/UserProfiles
        [HttpPost]
        public async Task<ActionResult<UserProfile>> PostUserProfile(UserProfile userProfile)
        {
            _logger.LogInformation("Creating a new user profile");

            var currentUser = _userService.CurrentUser;

            if (userProfile.UserId != currentUser.Id || UserProfileExistsForUser(currentUser.Id))
            {
                _logger.LogWarning("Attempt to create multiple profiles or profiles for other users");
                return BadRequest("You can only create your own profile.");
            }

            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserProfile", new { id = userProfile.UserProfileId }, userProfile);
        }

        private bool UserProfileExistsForUser(string userId)
        {
            return _context.UserProfiles.Any(e => e.UserId == userId);
        }
    }
}
