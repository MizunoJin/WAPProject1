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

        public UserProfilesController(TenderContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        // GET: api/UserProfiles
        [HttpGet]
        public async Task<ActionResult<UserProfile>> GetUserProfile()
        {
            var currentUser = _userService.CurrentUser;
            if (currentUser == null)
            {
                return BadRequest("User not found.");
            }

            var currentUserId = _userService.CurrentUser.UserId;

            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(up => up.UserId == currentUserId);

            if (userProfile == null)
            {
                return NotFound();
            }

            return userProfile;
        }

        // PUT: api/UserProfiles
        [HttpPut]
        public async Task<IActionResult> PutUserProfile(UserProfile userProfile)
        {
            var currentUser = _userService.CurrentUser;
            if (currentUser == null)
            {
                return BadRequest("User not found.");
            }

            var currentUserId = currentUser.UserId;

            if (userProfile.UserId != currentUserId)
            {
                return BadRequest("You can only update your own profile.");
            }

            if (!UserProfileExists(userProfile.UserProfileId))
            {
                return NotFound();
            }

            _context.Entry(userProfile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        // POST: api/UserProfiles
        [HttpPost]
        public async Task<ActionResult<UserProfile>> PostUserProfile(UserProfile userProfile)
        {
            var currentUser = _userService.CurrentUser;
            if (currentUser == null)
            {
                return BadRequest("User not found.");
            }

            var currentUserId = _userService.CurrentUser.UserId;

            // Prevent users from creating multiple profiles or profiles for other users
            if (userProfile.UserId != currentUserId || UserProfileExistsForUser(currentUserId))
            {
                return BadRequest("You can only create your own profile.");
            }

            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserProfile", new { id = userProfile.UserProfileId }, userProfile);
        }

        private bool UserProfileExists(int id)
        {
            return _context.UserProfiles.Any(e => e.UserProfileId == id);
        }

        private bool UserProfileExistsForUser(int userId)
        {
            return _context.UserProfiles.Any(e => e.UserId == userId);
        }
    }
}
