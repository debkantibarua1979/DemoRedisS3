using DemoRedisS3v2.Dtos;
using DemoRedisS3v2.Services.Interfaces;

namespace DemoRedisS3v2.Services.Implementations;


using System.Text.Json;

public class LogStorageService : ILogStorageService
{
    private readonly string _dir;

    public LogStorageService(IConfiguration config)
    {
        _dir = config["LogStorage:Directory"] ?? "logstack-data";
        if (!Directory.Exists(_dir)) Directory.CreateDirectory(_dir);
    }

    public async Task AppendLogAsync(string key, FlightSearchResultEnvolope.FlightSearchResultEnvelope data)
    {
        var safeKey = key.Replace(":", "_");
        var path = Path.Combine(_dir, $"{safeKey}_{DateTime.UtcNow:yyyyMMddHHmmss}.json");

        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(path, json);
    }
}
