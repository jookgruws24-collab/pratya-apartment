using Application.Interfaces;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;

namespace Infrastructure.Storage;

// เก็บไฟล์ลง Azure Blob Storage แล้วคืน url สาธารณะของ blob
// ใช้ตอน deploy จริง (ตั้งค่า Storage:Provider = "Azure")
public class AzureBlobStorageService : IFileStorageService
{
    private readonly StorageSettings _settings;

    public AzureBlobStorageService(IOptions<StorageSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<string> SaveAsync(
        Stream content, string fileName, string contentType)
    {
        var containerClient = new BlobContainerClient(
            _settings.AzureConnectionString, _settings.AzureContainer);

        // สร้าง container ถ้ายังไม่มี และตั้งให้เปิดอ่าน blob ได้แบบสาธารณะ
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var blobName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
        var blobClient = containerClient.GetBlobClient(blobName);

        var headers = new BlobHttpHeaders { ContentType = contentType };
        await blobClient.UploadAsync(
            content,
            new BlobUploadOptions { HttpHeaders = headers });

        return blobClient.Uri.ToString();
    }
}
