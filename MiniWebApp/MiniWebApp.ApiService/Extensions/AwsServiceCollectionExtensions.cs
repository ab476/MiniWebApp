using Amazon.S3;
using Microsoft.Extensions.Options;
using MiniWebApp.ApiService.Options;
using MiniWebApp.ApiService.Services;

namespace MiniWebApp.ApiService.Extensions;

public static class AwsServiceCollectionExtensions
{
    public static IServiceCollection AddAwsS3Service(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AwsOptions>(
            configuration.GetSection("AWS"));

        services.AddSingleton<IAmazonS3>(sp =>
        {
            var opt = sp.GetRequiredService<IOptions<AwsOptions>>().Value;
            var config = new AmazonS3Config
            {
                ServiceURL = "http://localhost:4566",
                ForcePathStyle = true
            };
            return new AmazonS3Client(
                opt.AccessKey,
                opt.SecretKey,
                config
            );
        });

        services.AddScoped<IS3Service, S3Service>();

        // runs once on app start
        services.AddHostedService<S3StartupService>();

        return services;
    }
}
