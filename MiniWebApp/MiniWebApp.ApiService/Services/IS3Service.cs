namespace MiniWebApp.ApiService.Services;

public interface IS3Service
{
    Task UploadAsync(string key, Stream data, string contentType);
    Task<Stream> DownloadAsync(string key);
    Task DeleteAsync(string key);
}

