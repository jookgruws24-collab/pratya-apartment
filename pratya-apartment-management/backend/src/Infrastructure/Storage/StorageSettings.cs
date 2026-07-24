namespace Infrastructure.Storage;

public class StorageSettings
{
    // "Local" = เก็บลงโฟลเดอร์ในเครื่อง (เหมาะกับตอน dev)
    // "Azure" = เก็บลง Azure Blob Storage (เหมาะกับตอน deploy)
    public string Provider { get; set; } = "Local";

    // ใช้ตอน Provider = Azure
    public string AzureConnectionString { get; set; } = string.Empty;

    public string AzureContainer { get; set; } = "uploads";
}
