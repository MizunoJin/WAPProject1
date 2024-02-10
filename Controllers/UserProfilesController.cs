using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WADProject1.Models; 

namespace WADProject1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController : ControllerBase
    {
        private readonly TenderContext _context;

        public UserProfilesController(TenderContext context)
        {
            _context = context;
        }

        // GET: api/UserProfiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserProfile>>> GetUserProfiles()
        {
            return await _context.UserProfiles.Include(up => up.User).ToListAsync();
        }

        // GET: api/UserProfiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfile>> GetUserProfile(int id)
        {
            var userProfile = await _context.UserProfiles.Include(up => up.User).FirstOrDefaultAsync(up => up.UserProfileId == id);

            if (userProfile == null)
            {
                return NotFound();
            }

            return userProfile;
        }

        // PUT: api/UserProfiles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserProfile(int id, UserProfile userProfile)
        {
            if (id != userProfile.UserProfileId)
            {
                return BadRequest();
            }

            _context.Entry(userProfile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserProfileExists(id))
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

        // POST: api/UserProfiles
        [HttpPost]
        public async Task<ActionResult<UserProfile>> PostUserProfile(UserProfile userProfile)
        {
            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserProfile), new { id = userProfile.UserProfileId }, userProfile);
        }

        // DELETE: api/UserProfiles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserProfile(int id)
        {
            var userProfile = await _context.UserProfiles.FindAsync(id);
            if (userProfile == null)
            {
                return NotFound();
            }

            _context.UserProfiles.Remove(userProfile);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserProfileExists(int id)
        {
            return _context.UserProfiles.Any(e => e.UserProfileId == id);
        }
    }
}
