using Azure.Storage.Blobs;
using System;

namespace DockerDemo_BlobVolume
{
    internal class Program
    {
        private static string connectionString = "";
        private static string containerName = "data";
        private static string blobName = "Course.json";

        private static string localBlob = @"\app\Course.json";

        static void Main(string[] args)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            blobClient.DownloadTo(localBlob);

            Console.WriteLine("Blob downloaded");
        }
    }
}
