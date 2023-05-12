#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ABABI.Models;
using ABABI.Utilities;
using System.Collections;
namespace ABABI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ABABIDBContext _context;
        public UserController(ABABIDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDataModel>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(long id)
        {
            if (!UserExists(id))
            {
                return null;
            }
            var testModel = await _context.Users.FindAsync(id);
            var user = ControllerLogic.decryptUser(testModel);
            return user;
        }
        [HttpGet("/GetUserFromUsername/{username}")]
        public async Task<ActionResult<User>> GetUser(string username)
        {
            bool test = await doesUsernameExist(username);
            if (!test)
            {
                return null;
            }
            var testModel = await _context.Users.Where(o=>o.Username.Equals(username)).FirstAsync();
            var user = ControllerLogic.decryptUser(testModel);
            return user;
        }

        [HttpGet("/login/{username}/{password}")]
        public async Task<ActionResult<User>> LoginUser(String username, String password)
        {
            var userFromUserName = await _context.Users.Where(a => a.Username == username).FirstOrDefaultAsync();
            var id = userFromUserName.Id;
            var testModel = await _context.Users.FindAsync(id);
            await _context.SaveChangesAsync();
            var user = ControllerLogic.decryptUser(testModel);

            if (user.Password == password)
            {
                return user;
            } else
            {
                return null;
            }

        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            var upload = ControllerLogic.encryptUser(user);
            _context.Users.Add(upload);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);

        }

        [HttpGet("/register/{username}/{password}/{email}/{name}")]
        public async Task<ActionResult<User>> RegisterUser(String username, String password, String email, String name)
        {
            User user = new User { Name = username, Password = password, Email = email, Username = username, AvatarId = 0, WhiteList = false, LastLoginTime = DateTime.Now};

            var upload = ControllerLogic.encryptUser(user);
            _context.Users.Add(upload);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);

        }

        [HttpGet("/avatar/{userId}/{avatarId}")]
        public async Task<ActionResult> UpdateAvatarUser(long userId, int avatarId)
        {
            var model = await _context.Users.FindAsync(userId);
            model.AvatarId = avatarId;
            await _context.SaveChangesAsync();
            return Accepted();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var testModel = await _context.Users.FindAsync(id);
            if (testModel == null)
            {
                return NotFound();
            }

            _context.Users.Remove(testModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
        [HttpGet("exists/{id}")]
        public async Task<bool> doesUserExists(long id)
        {
            return await _context.Users.AnyAsync(e => e.Id == id);
        }
        [HttpGet("usernameExists/{username}")]
        public async Task<bool> doesUsernameExist(String username)
        {
            return await _context.Users.AnyAsync(e => Equals(e.Username, username));
        }
        [HttpGet("whitelistUser/{username}")]
        public async Task<bool> whitelistUser(String username)
        {
            var user = await _context.Users.Where(e => Equals(e.Username, username)).FirstOrDefaultAsync();
            if (user != null && user.Username.Length >0)
            {
                user.WhiteList = true;
                await _context.SaveChangesAsync();
            }

            return true;
        }
        [HttpGet("updateName/{userId}/{name}")]
        public async Task<ActionResult> UpdateName(long userId, string name)
        {
            var model = await _context.Users.FindAsync(userId);
            model.Name = name;
            await _context.SaveChangesAsync();
            return Accepted();
        }
        [HttpGet("randomUser")]
        public async Task<ActionResult<User>> GetRabdomWhiteListedUser()
        {
            var whiteListedUsers = await _context.Users.Where(e => e.WhiteList).ToListAsync();
            if (whiteListedUsers.Count > 0)
            {
                Random random = new Random();
                int random_idx = random.Next(0, whiteListedUsers.Count);
                var user = ControllerLogic.decryptUser(whiteListedUsers[random_idx]);
                user.Password = "";
                user.Email = "";
                return user;
            }
            return NoContent();

        }
    }
}
