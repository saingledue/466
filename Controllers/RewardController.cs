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
    public class RewardController : ControllerBase
    {
        private readonly fundDBContext _context;

        public RewardController(fundDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reward>>> GetRewards()
        {
            return await _context.Rewards.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reward>> GetReward(long id)
        {
            var reward = await _context.Rewards.FindAsync(id);

            if (reward == null)
            {
                return NotFound();
            }

            return reward;
        }

        
        [HttpPost]
        public async Task<ActionResult<Reward>> PostReward(Reward reward)
        {
            _context.Rewards.Add(reward);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReward), new { id = reward.Id }, reward);

        }
        [HttpGet("contents/{imageURL}/{pointsWorth}")]
        public async Task<ActionResult<Reward>> PostReward(string imageURL, int pointsWorth)
        {
            Reward reward = new Reward {Image=imageURL,PointsWorth=pointsWorth };
            _context.Rewards.Add(reward);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReward), new { id = reward.Id }, reward);

        }
    }
}
