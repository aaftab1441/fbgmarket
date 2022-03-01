using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBG.Market.Databackfiller.BlobStorage
{
    public class AzureBlobClient
    {
        private const string connectionString = "DefaultEndpointsProtocol=https;AccountName=fbgmarketdevelopement;AccountKey=wjDz2S4eEnTXIAhI6h+dyW+vd+xHZ3/r7bs09idX4bpX9YgX9H4f4F+0DyQvklDDgSu9I71SeTOy+AStP+/mAQ==;EndpointSuffix=core.windows.net";
        private const string basePath = "DownloadedImages/";
        private const string containerName = "development";

        public void UploadBlob(string filename, string prefix)
        {
            try
            {
                // Create a BlobServiceClient object which will be used to create a container client
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                // Create the container and return a container client object
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                // Create a local file in the ./data/ directory for uploading and downloading
                string localFilePath = Path.Combine(basePath, filename);

                // Get a reference to a blob
                BlobClient blobClient = containerClient.GetBlobClient(prefix + "/" + filename);

                // Upload data from the local file
                blobClient.Upload(localFilePath, true);

            }
            catch (Exception)
            {

                throw;
            }

        }

        public void ListAllBlobs()
        {
            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            // Create the container and return a container client object
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            Console.WriteLine("Listing blobs...");

            // List all blobs in the container
            foreach (var blobItem in containerClient.GetBlobs())
            {
                Console.WriteLine("\t" + blobItem.Name);
            }
        }
    }
}
