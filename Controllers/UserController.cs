#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SEWebApp.Models;
using SEWebApp.Utilities;
using System.Collections;
namespace SEWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly fundDBContext _context;
        public UserController(fundDBContext context)
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
            if (DateTime.Now.Subtract(testModel.LastLoginTime).TotalDays >= 1) {
                testModel.LastLoginTime = DateTime.Now;
                testModel.GiftablePoints = 50;
            }
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

        [HttpGet("/getPrivacy/{id}")]
        public async Task<int> UserPrivacy(long id)
        {
            if (!UserExists(id))
            {
                return -1;
            }
            var testModel = await _context.Users.FindAsync(id);
            var user = ControllerLogic.decryptUser(testModel);
            return user.PrivacySetting;
        }

        [HttpGet("/getLeaderboard/{userid}")]
        public async Task<IEnumerable<LeaderboardPosition>> UserLeaderboard(long userid)
        {
            // grab the users that should be displayed and order by how many total points they have
            var encryptedOrderedUsers = await _context.Users.OrderByDescending(s => s.TotalPoints).Where(a => a.PrivacySetting == 1 || a.PrivacySetting == 3 || a.Id == userid).ToListAsync();

            // decrypt all the users
            var users = new ArrayList();
            foreach (UserDataModel u in encryptedOrderedUsers)
            {
                users.Add(Utilities.ControllerLogic.decryptUser(u));
            }

            Boolean leaderboardHasUser = false;
            // create leaderboard objects based on position and points, keep track of if
            // the logged in user has showed up on leaderboard
            var sortedLeaderboard = new ArrayList();
            foreach (var user in users.OfType<User>())
            {
                if (sortedLeaderboard.Count < 5)
                {
                    LeaderboardPosition lp = new LeaderboardPosition { username = user.Username, totalPoints = user.TotalPoints, position = sortedLeaderboard.Count + 1, avatarId = user.AvatarId };
                    sortedLeaderboard.Add(lp);
                    if (user.Id == userid)
                    {
                        leaderboardHasUser = true;
                    }
                }
            }

            // if the logged in user isn't in the top 5, show them their position
            if (!leaderboardHasUser)
            {
                UserDataModel thisUserDataModel = await _context.Users.FindAsync(userid);
                if (thisUserDataModel != null)
                {
                    User thisUser = Utilities.ControllerLogic.decryptUser(thisUserDataModel);

                    int userIndex = -1;
                    int index = 1;
                    foreach (User u in users)
                    {
                        if (u.Id == userid)
                        {
                            userIndex = index;
                        }
                        index = index + 1;
                    }

                    LeaderboardPosition lp = new LeaderboardPosition { username = thisUser.Username, totalPoints = thisUser.TotalPoints, position = userIndex, avatarId = thisUser.AvatarId };
                    sortedLeaderboard.Add(lp);
                }
            }
            return sortedLeaderboard.Cast<LeaderboardPosition>().AsEnumerable<LeaderboardPosition>();
        }

        [HttpGet("/privacyUpdate/{userId}/{privacySetting}")]
        public async Task<ActionResult> UpdateUserPrivacy(long userId, int privacySetting)
        {
            var model = await _context.Users.FindAsync(userId);
            model.PrivacySetting = privacySetting;
            await _context.SaveChangesAsync();
            return Accepted();
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
            User user = new User { Name = username, Password = password, Email = email, Username = username, AvatarId = 0, PrivacySetting = 0, SpendablePoints = 0, GiftablePoints = 50, TotalPoints = 0, LastLoginTime = DateTime.Now, LastMessageSent=DateTime.Now };

            var upload = ControllerLogic.encryptUser(user);
            _context.Users.Add(upload);
            await _context.SaveChangesAsync();

            String subject = "Welcome to block";
            String text = @"Hey " + username + ",\n\nWe are thrilled to welcome you to the block positivity messaging service! \n\n- The block team";
            ControllerLogic.sendEmail(username, email, subject, text);

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

        [HttpGet("messageInactiveUsers")]
        public async Task<bool> messageInactiveUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                foreach (UserDataModel udm in users)
                {
                    if (udm.LastMessageSent.ToShortDateString().Equals((DateTime.Now - TimeSpan.FromDays(14)).ToShortDateString()))
                    {
                        User u = ControllerLogic.decryptUser(udm);
                        String subject = "We've missed you!";
                        String text = @"Hey " + u.Username + ",\n\nWe've missed you!  It's been 14 days since you last used our service. Log back in to see what messages of positivity have been shared with you!\n\n- The block team";
                        ControllerLogic.sendEmail(u.Username, u.Email, subject, text);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
