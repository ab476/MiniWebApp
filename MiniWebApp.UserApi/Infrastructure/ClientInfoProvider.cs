using System.Net;
using UAParser; // Install-Package UAParser
using MaxMind.GeoIP2;

namespace MiniWebApp.UserApi.Infrastructure;

public class ClientInfo
{
    public IPAddress? IpAddress { get; set; }
    public string? DeviceInfo { get; set; }
    public string? Location { get; set; }
}
public interface IClientInfoProvider
{
    ClientInfo GetClientInfo();
}

public class ClientInfoProvider(IHttpContextAccessor httpContextAccessor) : IClientInfoProvider
{
    public ClientInfo GetClientInfo()
    {
        var context = httpContextAccessor.HttpContext;
        if (context == null) return new ClientInfo();

        var userAgent = context.Request.Headers.UserAgent.ToString();
        var uaParser = Parser.GetDefault();
        var client = uaParser.Parse(userAgent);

        return new ClientInfo
        {
            // 1. IP Address (Resolved via Forwarded Headers middleware)
            IpAddress = context.Connection.RemoteIpAddress,

            // 2. Device Info (Structured via UA Parser)
            DeviceInfo = $"{client.OS} | {client.Device} | {client.UA}",

            // 3. Location (Requires a GeoIP service or local DB)
            Location = GetLocationFromIp(context.Connection.RemoteIpAddress)
        };
    }

    private string GetLocationFromIp(IPAddress? ip)
    {
        if (ip == null || IPAddress.IsLoopback(ip)) return "Localhost";

        try
        {
            // In a real app, inject DatabaseReader as a Singleton for performance
            // Path to the downloaded GeoLite2-City.mmdb file
            using var reader = new DatabaseReader("App_Data/GeoLite2-City.mmdb");

            var city = reader.City(ip);

            string cityName = city.City.Name ?? "Unknown City";
            string countryName = city.Country.IsoCode ?? "Unknown Country";

            return $"{cityName}, {countryName}";
        }
        catch (Exception)
        {
            // MaxMind throws an exception if the IP is not in the database
            return "Unknown Location";
        }
    }
}