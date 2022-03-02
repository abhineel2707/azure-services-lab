using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;

namespace KeyVaultDemo
{
    internal class Program
    {
        // Properties to access the application object
        private static string tenantId = "<APPLICATION OBJECT TENANT ID>";
        private static string clientId = "<APPLICATION OBJECT CLIENT ID>";
        private static string clientSecret = "<APPLICATION OBJECT CLIENT SECRET>";

        private static string keyVaultURL = @"<KEY VAULT URL>";
        private static string secretName = "dbpassword";
        static void Main(string[] args)
        {
            ClientSecretCredential clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            SecretClient secretClient = new SecretClient(new Uri(keyVaultURL), clientSecretCredential);

            var secret = secretClient.GetSecret(secretName);

            Console.WriteLine($"The value of secret is {secret.Value.Value}");

            Console.ReadLine();
        }
    }
}
