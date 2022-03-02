using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using System;
using System.Text;

namespace KeyVault_EncryptionKeysDemo
{
    internal class Program
    {
        // Properties to access the application object
        private static string tenantId = "<APPLICATION OBJECT TENANT ID>";
        private static string clientId = "<APPLICATION OBJECT CLIENT ID>";
        private static string clientSecret = "<APPLICATION OBJECT CLIENT SECRET>";

        private static string keyVaultUrl = @"<KEY VAULT URL>";
        private static string keyName = "newkey";
        private static string textToEncrypt = "This text needs to be encrypted.";
        static void Main(string[] args)
        {
            ClientSecretCredential clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);

            KeyClient keyClient = new KeyClient(new Uri(keyVaultUrl), clientSecretCredential);

            var key = keyClient.GetKey(keyName);

            var cryptoClient = new CryptographyClient(key.Value.Id, clientSecretCredential);

            // Encryption is done on the byte[]
            byte[] textToByte = Encoding.UTF8.GetBytes(textToEncrypt);
            EncryptResult encryptResult = cryptoClient.Encrypt(EncryptionAlgorithm.RsaOaep, textToByte);

            Console.WriteLine("The encrypted text is...");
            Console.WriteLine(Convert.ToBase64String(encryptResult.Ciphertext));

            byte[] cipherToBytes = encryptResult.Ciphertext;

            DecryptResult decryptResult = cryptoClient.Decrypt(EncryptionAlgorithm.RsaOaep, cipherToBytes);

            Console.WriteLine($"Decrypted text is {Encoding.UTF8.GetString(decryptResult.Plaintext)}");
        }
    }
}
