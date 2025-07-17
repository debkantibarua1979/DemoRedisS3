using DemoRedisS3v2.Dtos;

namespace DemoRedisS3v2.Services.Interfaces;

public interface IRedisCacheService
{
    string GenerateCacheKey(FlightSearchRequest request);
    Task<FlightSearchResultEnvolope.FlightSearchResultEnvelope?> GetAsync(string key);
    Task SetAsync(string key, FlightSearchResultEnvolope.FlightSearchResultEnvelope result);
}
