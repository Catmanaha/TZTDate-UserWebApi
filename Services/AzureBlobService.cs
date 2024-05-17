using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using TZTDate_UserWebApi.Options;
using TZTDate_UserWebApi.Services.Base;

namespace TZTDate_UserWebApi.Services;

public class AzureBlobService : IAzureBlobService
{
    private readonly BlobServiceClient blobServiceClient;
    private readonly BlobContainerClient blobContainerClient;
    private readonly BlobOption blobOptions;

    public AzureBlobService(IOptionsSnapshot<BlobOption> optionsSnapshot)
    {
        blobServiceClient = new BlobServiceClient(optionsSnapshot.Value.ConnectionString);
        blobContainerClient = blobServiceClient.GetBlobContainerClient(optionsSnapshot.Value.ContainerName);
        blobOptions = optionsSnapshot.Value;
    }

    public async Task<List<BlobContentInfo>> UploadFiles(List<IFormFile> files)
    {
        var azureResponse = new List<BlobContentInfo>();

        foreach (var file in files)
        {
            string fileName = file.FileName;
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                memoryStream.Position = 0;

                var client = await blobContainerClient.UploadBlobAsync(fileName, memoryStream);
                azureResponse.Add(client);
            }
        }

        return azureResponse;
    }

    public async Task UploadFile(Stream file, string fileName)
    {
        await blobContainerClient.UploadBlobAsync(fileName, file);
    }

    public async Task<List<BlobItem>> GetBlob()
    {
        var items = new List<BlobItem>();

        var uploadedFiles = blobContainerClient.GetBlobsAsync();

        await foreach (BlobItem file in uploadedFiles)
        {
            items.Add(file);
        }

        return items;
    }

    public async Task DeleteBlob(string blobName)
    {
        await blobContainerClient.DeleteBlobAsync(blobName);
    }

    public BlobClient GetBlobItem(string blobName)
    {
        return blobContainerClient.GetBlobClient(blobName);
    }

    public string GetBlobItemSAS(string path)
    {
        var blobSasBuilder = new BlobSasBuilder
        {
            BlobContainerName = blobOptions.ContainerName,
            BlobName = path,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
        };

        blobSasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasToken = blobSasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(blobOptions.StorageAccountName, blobOptions.StorageKey)).ToString();
        var blobItem = this.GetBlobItem(path);
        var blobUriWithSas = new UriBuilder(blobItem.Uri)
        {
            Query = sasToken
        };

        return blobUriWithSas.ToString();
    }
}
