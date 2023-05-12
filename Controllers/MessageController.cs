#nullable disable
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
    public class MessageController : ControllerBase
    {
        private readonly ABABIDBContext _context;

        public MessageController(ABABIDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDataModel>>> GetMessages()
        {
            return await _context.Messages.ToListAsync();
        }

        [HttpGet("getSent/{id}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetAllSentMessages(long id)
        {
            var messages = await _context.Messages.Where(a => a.SenderId == id).ToListAsync();
            var returnModel = ControllerLogic.decryptMessageDataModel(messages);
            return returnModel.ToArray();
        }

        [HttpGet("getReceived/{id}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetAllReceivedMessages(long id)
        {
            var messages = await _context.Messages.Where(a => a.RecipientId == id).ToListAsync();
            var returnModel = ControllerLogic.decryptMessageDataModel(messages);
            return returnModel.ToArray();
        }

        [HttpGet("messagesBetween/{id1}/{recipientUsername}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetAllMessagesBetween(long id1, string recipientUsername)
        {
            var userToSendTo = await _context.Users.Where(o => (o.Username.Equals(recipientUsername))).FirstOrDefaultAsync();
            var messages = await _context.Messages.Where(a => ((a.RecipientId == id1 && a.SenderId == userToSendTo.Id) || (a.RecipientId == userToSendTo.Id && a.SenderId == id1))).ToListAsync();
            var returnModel = ControllerLogic.decryptMessageDataModel(messages);
            return returnModel.ToArray();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetMessage(long id)
        {
            var message = await _context.Messages.FindAsync(id);
            var returnModel = ControllerLogic.decryptSingleMessageDataModel(message);
            return returnModel;
        }
        [HttpGet("usersMessaged/{userID}")]
        public async Task<IEnumerable<UserDataModel>> GetUsersMessaged(long userID)
        {
            var messages = await _context.Messages.Where(o => (o.SenderId == userID || o.RecipientId == userID)).ToListAsync();
            var userIDs = new ArrayList();
            foreach (var message in messages)
            {
                var userMessaged = message.SenderId;
                if (userMessaged == userID)
                {
                    userMessaged = message.RecipientId;
                }
                userIDs.Add(userMessaged);
            }
            var users = await _context.Users.Where(o => (userIDs.Contains(o.Id))).ToListAsync();
            return users;
        }
        [HttpGet("/messageRead/{id}")]
        public async Task<ActionResult> MessageRead(long id)
        {
            var message = await _context.Messages.FindAsync(id);
            message.Read = true;
            await _context.SaveChangesAsync();
            return Accepted();
        }
        [HttpPost]
        public async Task<ActionResult<Message>> PostMessage(Message message)
        {
            message.Time = DateTime.Now;

            var saveModel = new MessageDataModel
            {
                Content = Encryption.encryptString(message.Content),
                SenderId = message.SenderId,
                Id = message.Id,
                Read = message.Read
            };
            _context.Messages.Add(saveModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMessage), new { id = message.Id }, message);

        }
        [HttpGet("{senderID}/{recipientUserName}/{messageContent}")]
        public async Task<ActionResult<Message>> SendMessage(long senderID, string recipientUserName, string messageContent)
        {
            var userToSendTo = await _context.Users.Where(o => (o.Username == recipientUserName)).FirstOrDefaultAsync();
            if (userToSendTo.Username.Length > 0)
            {
                Message message = new Message { Content = messageContent, RecipientId = userToSendTo.Id, SenderId = senderID, Time = DateTime.Now };
              
                //Save Message
                var saveModel = new MessageDataModel
                {
                    Content = Encryption.encryptString(message.Content),
                    SenderId = message.SenderId,
                    RecipientId = message.RecipientId,
                    Id = message.Id,
                    Read = message.Read,
                    Time = DateTime.Now,
                    
                };
                _context.Messages.Add(saveModel);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetMessage), new { id = message.Id }, message);
            }
            return new EmptyResult();

        }
        [HttpGet("lastMessages/{userID}")]
        public async Task<IEnumerable<LastMessage>> GetLastMessages(long userID)
        {
            var messages = await _context.Messages.Where(o => (o.SenderId == userID || o.RecipientId == userID)).ToListAsync();
            var userIDs = new ArrayList();
            //find all users you have messaged
            foreach (var message in messages)
            {
                var userMessaged = message.SenderId;
                if (userMessaged == userID)
                {
                    userMessaged = message.RecipientId;
                }
                userIDs.Add(userMessaged);
            }
            var users = await _context.Users.Where(o => (userIDs.Contains(o.Id))).ToListAsync();
            //get last message from each user along with read receipt
            var lastMessages = new ArrayList();
            foreach (var user in users)
            {
                var lastMessage = await _context.Messages.Where(o => ((o.SenderId == userID && o.RecipientId == user.Id) || (o.RecipientId == userID && o.SenderId == user.Id))).OrderBy(o=>o.Time).FirstAsync();
                LastMessage lastMessageObject = new LastMessage { lastMessage = Utilities.Encryption.decrpytString(lastMessage.Content),Read=lastMessage.Read, UserID = user.Id, Username = user.Username};
                lastMessages.Add(lastMessageObject);
            }
            return lastMessages.Cast<LastMessage>().ToList();
        }
    }
}
