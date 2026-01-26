using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using MiniWebApp.ApiService.Options;

namespace MiniWebApp.ApiService.Services;

public class S3Service(IAmazonS3 s3, IOptions<AwsOptions> opt) : IS3Service
{
    private readonly AwsOptions _opt = opt.Value;

    public async Task UploadAsync(string key, Stream data, string contentType)
    {
        var req = new PutObjectRequest
        {
            BucketName = _opt.BucketName,
            Key = key,
            InputStream = data,
            ContentType = contentType
        };

        await s3.PutObjectAsync(req);
    }

    public async Task<Stream> DownloadAsync(string key)
    {
        var res = await s3.GetObjectAsync(_opt.BucketName, key);
        return res.ResponseStream;
    }

    public async Task DeleteAsync(string key)
    {
        await s3.DeleteObjectAsync(_opt.BucketName, key);
    }
}
