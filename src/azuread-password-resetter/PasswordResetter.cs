using Azure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureAdPasswordResetter
{
    public class PasswordResetter
    {
        private readonly ILogger<PasswordResetter> _logger;

        public PasswordResetter(ILogger<PasswordResetter> log)
        {
            _logger = log;
        }

        //add TimeTrigger to run the reset at 8am every Monday (ex. "0 0 8 * * 1")
        [FunctionName("TimeTriggerResetPasswords")]
        public async Task TimeTriggerResetPasswords([TimerTrigger("0 0 8 * * 1")] TimerInfo myTimer)
        {
            var res = await ResetAllPasswords();
            _logger.LogInformation($"Reset password function executed at: {DateTime.Now}");
        }

        //add HttpTrigger to test the function
        [FunctionName("HttpTriggerResetPasswords")]
        public async Task<IActionResult> HttpTriggerResetPasswords(
                       [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            var res = await ResetAllPasswords();
            _logger.LogInformation($"Reset password function executed manually at: {DateTime.Now}");
            return res;
        }

        private async Task<IActionResult> ResetAllPasswords()
        {
            //get the access token using the client credentials flow
            var scopes = new[] { "https://graph.microsoft.com/.default" };
            
            var tenantId = Environment.GetEnvironmentVariable("TenantId");
            var clientId = Environment.GetEnvironmentVariable("ClientId");
            var clientSecret = Environment.GetEnvironmentVariable("ClientSecret");

            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret, options);

            //create a graph client to access Graph data
            var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

            //read all users from the group
            var users = await ReadUsersFromGraph(graphClient, Environment.GetEnvironmentVariable("AzureADGroupName"));

            var result = new StringBuilder();
            foreach (var user in users)
            {
                //generate a new random password
                var newPassword = GenerateRandomPassword(8);
                result.AppendLine(await ResetUserPassword(graphClient, user.UserPrincipalName, newPassword));
            }

            //send an email with the list of users and their new passwords
            var recipientAddress = Environment.GetEnvironmentVariable("RecipientAddress");
            await MailHelper.SendMail(recipientAddress, "Password Reset Notification", result.ToString());

            return new OkObjectResult($"Passwords reset completed. Mail sent to: {recipientAddress} ");
        }
        private async Task<List<User>> ReadUsersFromGraph(GraphServiceClient graphClient, string groupName = null)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                //if there is a group name, read the group and get the members
                var groupResponse = await graphClient.Groups.GetAsync();
                var group = groupResponse.Value.FirstOrDefault(g => g.DisplayName == groupName);

                if (group== null)
                {
                    return null;
                }

                var members = await graphClient.Groups[group.Id].Members.GetAsync();
                var memberIds = members.Value.Select(m => m.Id).ToList();

                var userRes = await graphClient.Users.GetAsync();
                var users = userRes.Value.Where(u => memberIds.Any(m => m == u.Id));

                return users.ToList();
            }
            else
            {
                //if there is no group name, read all users
                var userList = await graphClient.Users.GetAsync();
                return userList.Value;
            }
        }
        private async Task<string> ResetUserPassword(GraphServiceClient graphClient, string userId, string newPassword)
        {
            var passwordProfile = new PasswordProfile
            {
                Password = newPassword,
                ForceChangePasswordNextSignIn = false
            };

            var user = new User
            {
                Id = userId,
                PasswordProfile = passwordProfile
            };

            try
            {
                await graphClient.Users[user.Id].PatchAsync(user);

                return $"The password for user '{userId}' has been reset to '{newPassword}'";

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving user: {ex.Message}");
                return $"Error resetting password for user '{userId}': {ex.Message}";
            }
            
        }
        private string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+-=[]{};:,.<>?";
            StringBuilder password = new StringBuilder();
            Random rnd = new Random();

            while (0 < length--)
            {
                password.Append(validChars[rnd.Next(validChars.Length)]);
            }

            return password.ToString();
        }
    }
}

