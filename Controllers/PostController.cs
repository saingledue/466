using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ABABI.Models;
using ABABI.Utilities;
using System.Text.RegularExpressions;
using System.Collections;
namespace ABABI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ABABIDBContext _context;

        public PostController(ABABIDBContext context)
        {
            _context = context;
        }

        [HttpGet("/Message/globalMessages")]
        public async Task<ActionResult<IEnumerable<Post>>> GetAllGlobalMessages()
        {
            return await _context.Posts.ToListAsync();
        }

        [HttpGet("/Message/globalMessage/{senderID}/{messageContent}")]
        public async Task<ActionResult<Post>> PostGlobalMessage(long senderID, string messageContent)
        {
            var sending_user = await _context.Users.Where(o => (o.Id == senderID)).FirstOrDefaultAsync();
            var newGUID = Guid.NewGuid();
            if (sending_user != null && sending_user.Username.Length > 0)
            {
                var saveModel = new Post
                {
                    PostId = newGUID,
                    PostAuthorId = senderID,
                    ParentPostId = newGUID,
                    Score = 0,
                    Text = messageContent,
                    Time = DateTime.Now,


                };
                _context.Posts.Add(saveModel);
                await _context.SaveChangesAsync();
                return saveModel;
            }
            return new EmptyResult();

        }
        [HttpGet("/Message/globalMessageReply/{senderID}/{postGUID}/{messageContent}")]
        public async Task<ActionResult<Post>> PostGlobalMessage(long senderID, string postGUID, string messageContent)
        {
            var sending_user = await _context.Users.Where(o => (o.Id == senderID)).FirstOrDefaultAsync();
            var newGUID = Guid.NewGuid();
            if (sending_user != null && sending_user.Username.Length > 0)
            {
                var saveModel = new Post
                {
                    PostId = newGUID,
                    PostAuthorId = senderID,
                    ParentPostId = Guid.Parse(postGUID),
                    Score = 0,
                    Text = messageContent,
                    Time = DateTime.Now,


                };
                _context.Posts.Add(saveModel);
                await _context.SaveChangesAsync();
                return saveModel;
            }
            return new EmptyResult();

        }
        [HttpGet("/Message/globalMessage/increaseScore/{postGUID}")]
        public async Task<ActionResult> IncrementScore(string postGUID)
        {
            var post = await _context.Posts.Where(o => (o.PostId == Guid.Parse(postGUID))).FirstOrDefaultAsync();
            if (post != null)
            {

                post.Score++;
                await _context.SaveChangesAsync();
            }
            return Accepted();

        }
        [HttpGet("/Message/globalMessage/removePost/{postGUID}")]
        public async Task<ActionResult> RemovePost(string postGUID)
        {
            var post = await _context.Posts.Where(o => (o.PostId == Guid.Parse(postGUID))).FirstOrDefaultAsync();
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
            return Accepted();

        }

    }
}
