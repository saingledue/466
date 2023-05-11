using SEWebApp.Models;
using SEWebApp.Utilities;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
using System.Text.RegularExpressions;
namespace SEWebApp.Utilities
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
                Name = Encryption.decrpytString(encryptedUser.Name),
                Username = encryptedUser.Username,
                AvatarId = encryptedUser.AvatarId,
                LastLoginTime = encryptedUser.LastLoginTime,
                PrivacySetting = encryptedUser.PrivacySetting,
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
                Name = Encryption.encryptString(unencryptedUser.Name),
                Username = unencryptedUser.Username,
                AvatarId = unencryptedUser.AvatarId,
                LastLoginTime = unencryptedUser.LastLoginTime,
                PrivacySetting = unencryptedUser.PrivacySetting,
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

        public static Boolean sendEmail(String username, String email, String subject, String text)
        {
            Boolean success = true;
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("block", "blockofficial2022@gmail.com"));
            message.To.Add(new MailboxAddress(username, email));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = text
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect("smtp.gmail.com", 587, false);

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate("blockofficial2022@gmail.com", "blockpassword123*");

                    client.Send(message);
                    client.Disconnect(true);
                }  catch (Exception ex)
                {
                    success = false;
                }
            }
            return success;
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
