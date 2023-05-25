using Azure.Communication.Email;
using System;
using System.Threading.Tasks;

namespace AzureAdPasswordResetter
{
    public static class MailHelper
    {

        public static async Task SendMail(string recipient, string subject, string body)
        {
            var connectionString = Environment.GetEnvironmentVariable("CommunicationServiceConnectionString");
            var emailClient = new EmailClient(connectionString);

            var emailContent = new EmailContent(subject)
            {
                PlainText = body,
                Html = ""
            };
            var sender = Environment.GetEnvironmentVariable("SenderAddress");

            var emailMessage = new EmailMessage(sender, recipient,  emailContent);

            try
            {
                var sendEmailResult = await emailClient.SendAsync(Azure.WaitUntil.Started, emailMessage);

                string messageId = sendEmailResult.Id;
                if (!string.IsNullOrEmpty(messageId))
                {
                    Console.WriteLine($"Email sent, MessageId = {messageId}");
                }
                else
                {
                    Console.WriteLine($"Failed to send email.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in sending email, {ex.Message}");
            }
        }
    }
}
