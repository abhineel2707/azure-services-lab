using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using System;
using System.IO;

namespace ManagedIdentityDemo
{
    internal class Program
    {
        private static string blobUrl = @"<BLOB URL>";
        private static string downloadLocation = @"C:\tmp\";
        static void Main(string[] args)
        {
            TokenCredential tokenCredential = new DefaultAzureCredential();
            Uri blobUri = new Uri(blobUrl);

            Console.WriteLine(tokenCredential.ToString());

            BlobClient blobClient = new BlobClient(blobUri, tokenCredential);

            FileStream fs = new FileStream(downloadLocation + $"{blobClient.Name}", FileMode.OpenOrCreate, FileAccess.Write);
            using (StreamWriter writer = new StreamWriter(fs))
            {
                var res = blobClient.DownloadContent().Value;
                //res.Content.ToArray();
                writer.Write(res.Content);
            }

            Console.WriteLine("File download completed.");

            Console.ReadLine();
        }
    }
}