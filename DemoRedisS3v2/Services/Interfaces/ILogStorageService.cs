using DemoRedisS3v2.Dtos;

namespace DemoRedisS3v2.Services.Interfaces;


public interface ILogStorageService
{
    Task AppendLogAsync(string key, FlightSearchResultEnvolope.FlightSearchResultEnvelope data);
}
