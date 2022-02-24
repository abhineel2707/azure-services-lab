using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using System;
using System.Collections.Generic;
using System.IO;

namespace BlobServiceDemo
{
    internal class Program
    {
        private static string connectionString = "";
        private static string containerName = "mydata";
        private static string blobName = "Program.cs";
        private static string location = @"D:\Azure Az 204\Azure Services Code Repo\src\Azure Storage\BlobServiceDemo\Program.cs";
        private static string downloadLocation = @"C:\tmp\";

        static void Main(string[] args)
        {
            BlobUploadAndDownload();

            ReadBlob();

            DownloadBlob();

        }

        private static void BlobUploadAndDownload()
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            #region Create Container

            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            bool isExist = containerClient.Exists();
            if (!isExist)
            {
                containerClient.Create();
                //container.CreateIfNotExists();
                Console.WriteLine("Container Created...");
            }

            #endregion

            #region Create blob

            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            var isBlobExist = blobClient.Exists();
            if (!isBlobExist)
            {
                blobClient.Upload(location);
                Console.WriteLine("Blob has been uploaded successfully.");

            }
            #endregion

            #region List All Blobs

            var blobs = containerClient.GetBlobs();
            foreach (var blob in blobs)
            {
                Console.WriteLine(blob.Name);
            }

            #endregion

            #region Download Blobs

            FileStream fs = new FileStream(downloadLocation + $"{blobClient.Name}", FileMode.OpenOrCreate, FileAccess.Write);
            using (StreamWriter writer = new StreamWriter(fs))
            {
                var res = blobClient.DownloadContent().Value;
                //res.Content.ToArray();
                writer.Write(res.Content);
            }
            #endregion

            #region Blob Properties

            BlobProperties properties = blobClient.GetProperties();

            Console.WriteLine($"The access tier of the blob is: {properties.AccessTier}");
            Console.WriteLine($"The size of the blob is {properties.ContentLength}");

            #endregion

            #region Blob Metadata

            IDictionary<string, string> metadata = properties.Metadata;

            foreach (var item in metadata)
            {
                Console.WriteLine($"{item.Key} : {item.Value}");
            }

            #endregion
        }

        private static Uri GenerateSAS()
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            bool isExist = containerClient.Exists();
            if (!isExist)
            {
                containerClient.Create();
            }

            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            var isBlobExist = blobClient.Exists();
            if (!isBlobExist)
            {
                blobClient.Upload(location);
            }

            BlobSasBuilder builder = new BlobSasBuilder()
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b"
            };

            builder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.List);

            builder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);

            return blobClient.GenerateSasUri(builder);
        }

        private static void ReadBlob()
        {
            Uri blobUri = GenerateSAS();

            BlobClient blobClient = new BlobClient(blobUri);

            FileStream fs = new FileStream(downloadLocation + $"{blobClient.Name}", FileMode.OpenOrCreate, FileAccess.Write);
            using (StreamWriter writer = new StreamWriter(fs))
            {
                var res = blobClient.DownloadContent().Value;
                //res.Content.ToArray();
                writer.Write(res.Content);
            }
        }

        private static void DownloadBlob()
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            bool isExist = containerClient.Exists();
            if (!isExist)
            {
                containerClient.Create();
                Console.WriteLine("Container Created...");
            }

            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            var isBlobExist = blobClient.Exists();
            if (!isBlobExist)
            {
                blobClient.Upload(location);
                Console.WriteLine("Blob has been uploaded successfully.");
            }

            MemoryStream memoryStream = new MemoryStream();
            blobClient.DownloadTo(memoryStream);
            memoryStream.Position = 0;
            StreamReader reader = new StreamReader(memoryStream);
            Console.WriteLine(reader.ReadToEnd());

            // Before writing let us acquire the lease
            BlobLeaseClient blobLeaseClient = blobClient.GetBlobLeaseClient();
            BlobLease blobLease = blobLeaseClient.Acquire(TimeSpan.FromSeconds(30));
            Console.WriteLine($"The lease id is {blobLease.LeaseId}");

            StreamWriter writer = new StreamWriter(memoryStream);
            writer.Write("This is a change");
            writer.Flush();

            memoryStream.Position = 0;

            BlobUploadOptions blobUploadOptions = new BlobUploadOptions()
            {
                Conditions = new BlobRequestConditions()
                {
                    LeaseId = blobLease.LeaseId
                }
            };

            // 2nd parameter is to overwrite the blob if it already exists
            // blobClient.Upload(memoryStream, true);

            blobClient.Upload(memoryStream, blobUploadOptions);

            // Release the lease
            blobLeaseClient.Release();
        }
    }
}
