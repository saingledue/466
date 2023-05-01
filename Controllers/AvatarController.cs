#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SEWebApp.Models;
namespace SEWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvatarController : ControllerBase
    {
        private readonly fundDBContext _context;

        public AvatarController(fundDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Avatar>>> GetAvatars()
        {
            return await _context.Avatars.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Avatar>> GetAvatar(long id)
        {
            var avatar = await _context.Avatars.FindAsync(id);

            if (avatar == null)
            {
                return NotFound();
            }

            return avatar;
        }

        
        [HttpPost]
        public async Task<ActionResult<Avatar>> PostAvatar(Avatar avatar)
        {
            _context.Avatars.Add(avatar);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAvatar), new { id = avatar.Id }, avatar);

        }
        [HttpGet("{imageURL}")]
        public async Task<ActionResult<Avatar>> PostAvatar(string imageURL)
        {
            Avatar avatar = new Avatar {Image=imageURL};
            _context.Avatars.Add(avatar);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAvatar), new { id = avatar.Id }, avatar);

        }
    }
}
