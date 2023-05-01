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
    public class EmojiController : ControllerBase
    {
        private readonly fundDBContext _context;

        public EmojiController(fundDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Emoji>>> GetEmojis()
        {
            return await _context.Emojis.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Emoji>> GetEmoji(long id)
        {
            var emoji = await _context.Emojis.FindAsync(id);

            if (emoji == null)
            {
                return NotFound();
            }

            return emoji;
        }

        
        [HttpPost]
        public async Task<ActionResult<Emoji>> PostEmoji(Emoji emoji)
        {
            _context.Emojis.Add(emoji);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmoji), new { id = emoji.Id }, emoji);

        }
    }
}
