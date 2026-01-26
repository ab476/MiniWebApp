using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.Extensions.Options;
using MiniWebApp.ApiService.Options;

namespace MiniWebApp.ApiService.Services;

public class S3StartupService(
    IAmazonS3 s3,
    IOptions<AwsOptions> opt,
    ILogger<S3StartupService> logger) : IHostedService
{
    private readonly AwsOptions _opt = opt.Value;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var bucket = _opt.BucketName;

        try
        {
            await s3.EnsureBucketExistsAsync(bucket);

            logger.LogInformation("S3 bucket '{Bucket}' exists.", bucket);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to verify or create S3 bucket '{Bucket}'.", bucket);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
