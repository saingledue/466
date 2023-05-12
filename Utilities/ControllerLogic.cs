using ABABI.Models;
using ABABI.Utilities;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
using System.Text.RegularExpressions;
namespace ABABI.Utilities
{
    public class ControllerLogic
    {
        public static User decryptUser(UserDataModel encryptedUser)
        {
            if (encryptedUser == null)
            {
                throw new ArgumentNullException("User object is null.");
            }

            var user = new User
            {
                Email = Encryption.decrpytString(encryptedUser.Email),
                Password = Encryption.decrpytString(encryptedUser.Password),
                Name = encryptedUser.Name,
                Username = encryptedUser.Username,
                AvatarId = encryptedUser.AvatarId,
                LastLoginTime = encryptedUser.LastLoginTime,
                WhiteList = encryptedUser.WhiteList,
                Id = encryptedUser.Id,
            };

            return user;
        }
        public static UserDataModel encryptUser(User unencryptedUser)
        {
            if (unencryptedUser == null)
            {
                throw new ArgumentNullException("User object is null.");
            }

            var user = new UserDataModel
            {
                Email = Encryption.encryptString(unencryptedUser.Email),
                Password = Encryption.encryptString(unencryptedUser.Password),
                Name = unencryptedUser.Name,
                Username = unencryptedUser.Username,
                AvatarId = unencryptedUser.AvatarId,
                LastLoginTime = unencryptedUser.LastLoginTime,
                WhiteList = unencryptedUser.WhiteList,
                Id = unencryptedUser.Id,
            };

            return user;
        }
        public static Message decryptSingleMessageDataModel(MessageDataModel encryptedMessage)
        {
            if (encryptedMessage == null)
            {
                throw new ArgumentNullException("You must supply a non-null message.");
            }

            var returnMessage = new Message
            {
                Content = Encryption.decrpytString(encryptedMessage.Content),
                SenderId = encryptedMessage.SenderId,
                RecipientId = encryptedMessage.RecipientId,
                Id = encryptedMessage.Id,
                Read = encryptedMessage.Read,
                Time = encryptedMessage.Time,
            };
            return returnMessage;
        }

        public static IEnumerable<Message> decryptMessageDataModel(IEnumerable<MessageDataModel> encryptedMessages)
        {
            if (encryptedMessages == null)
            {
                throw new ArgumentNullException("You must supply messages.");
            }

            var returnModel = new List<Message>();
            foreach (MessageDataModel message in encryptedMessages)
            {
                returnModel.Add(new Message
                {
                    Content = Encryption.decrpytString(message.Content),
                    SenderId = message.SenderId,
                    RecipientId = message.RecipientId,
                    Id = message.Id,
                    Read = message.Read,
                    Time = message.Time
                });
            };
            return returnModel;
        }

        public static int emojiCount(string input)
        {
            //Finds all non text unicode values
            Regex rgx = new Regex("[\x00-\x7F]");
            string emojis = rgx.Replace(input, "");
            return emojis.Length;
        }
    }
}
