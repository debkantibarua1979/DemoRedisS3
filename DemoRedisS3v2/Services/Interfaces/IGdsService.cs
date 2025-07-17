using DemoRedisS3v2.Dtos;

namespace DemoRedisS3v2.Services.Interfaces;

public interface IGdsService
{
    Task<List<FlightSearchResponse>> SearchFlightsAsync(FlightSearchRequest request);
}
