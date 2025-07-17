using DemoRedisS3v2.Dtos;
using DemoRedisS3v2.Services.Interfaces;

namespace DemoRedisS3v2.Services.Implementations;

using StackExchange.Redis;
using System.Text.Json;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase database;
    private readonly TimeSpan ttl;

    public RedisCacheService(IConnectionMultiplexer redis, IConfiguration config)
    {
        database = redis.GetDatabase();
        var ttlMinutes = int.TryParse(config["Redis:CacheTTLMinutes"], out var m) ? m : 60;
        ttl = TimeSpan.FromMinutes(ttlMinutes);
    }

    public string GenerateCacheKey(FlightSearchRequest r)
    {
        return $"flight:{r.Source}:{r.Destination}:{r.DepartureTime:yyyyMMddHHmm}-{r.ArrivalTime:yyyyMMddHHmm}";
    }

    public async Task<FlightSearchResultEnvolope.FlightSearchResultEnvelope?> GetAsync(string key)
    {
        var value = await database.StringGetAsync(key);
        return value.HasValue
            ? JsonSerializer.Deserialize<FlightSearchResultEnvolope.FlightSearchResultEnvelope>(value!)
            : null;
    }

    public async Task SetAsync(string key, FlightSearchResultEnvolope.FlightSearchResultEnvelope result)
    {
        var json = JsonSerializer.Serialize(result);
        await database.StringSetAsync(key, json, ttl);
    }
}
