using Azure.Identity;
using Azure.Storage.Blobs;
using System;
using System.IO;

namespace AppObjectsDemo
{
    internal class Program
    {
        private static string blobUrl = @"";

        // Properties to access the application object
        private static string tenantId = "<APPLICATION OBJECT TENANT ID>";
        private static string clientId = "<APPLICATION OBJECT CLIENT ID>";
        private static string clientSecret = "<APPLICATION OBJECT CLIENT SECRET>";

        static void Main(string[] args)
        {
            ClientSecretCredential clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);

            Uri blobUri = new Uri(blobUrl);

            BlobClient blobClient = new BlobClient(blobUri, clientSecretCredential);

            MemoryStream memoryStream = new MemoryStream();
            blobClient.DownloadTo(memoryStream);
            memoryStream.Position = 0;
            StreamReader reader = new StreamReader(memoryStream);
            Console.WriteLine(reader.ReadToEnd());

            Console.ReadLine();
        }
    }
}
