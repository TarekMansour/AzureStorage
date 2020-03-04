using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BlobStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public StorageController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("getblob/{blobContainerName}/{blobName}")]
        public BlobDownloadInfo GetBlob(string blobContainerName, string blobName)
        {
            var connectionString = _configuration["StorageConnectionString"];
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
            var blobClient = blobContainerClient.GetBlobClient(blobName);
            BlobDownloadInfo download = blobClient.Download(); blobClient.Download();

            return download;
        }

        [HttpGet("byte/{fileName}")]
        public ActionResult StreDownloadToByteArray(string fileName)
        {
            byte[] fileContent = GetFileContent(fileName);
            return File(fileContent, "application/zip", fileName);
        }

        public byte[] GetFileContent(string fileName)
        {
            var connectionString = _configuration["StorageConnectionString"];
            var storageAccount = CloudStorageAccount.Parse(connectionString);            
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("blobcontainertest");

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            blockBlob.FetchAttributesAsync();
            long fileByteLength = blockBlob.Properties.Length;
            byte[] fileContent = new byte[fileByteLength];
            for (int i = 0; i < fileByteLength; i++)
            {
                fileContent[i] = 0x20;
            }
            blockBlob.DownloadToByteArrayAsync(fileContent, 0);
            return fileContent;
        }
    }
}