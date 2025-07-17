using DemoRedisS3v2.Dtos;
using DemoRedisS3v2.Services.Interfaces;

namespace DemoRedisS3v2.Controllers;

using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class FlightSearchController : ControllerBase
{
    private readonly IRedisCacheService _redis;
    private readonly IGdsService _gds;
    private readonly ILogStorageService _logStorage;

    public FlightSearchController(
        IRedisCacheService redis,
        IGdsService gds,
        ILogStorageService logStorage)
    {
        _redis = redis;
        _gds = gds;
        _logStorage = logStorage;
    }

    [HttpPost]
    public async Task<IActionResult> SearchFlights([FromBody] FlightSearchRequest request)
    {
        var key = _redis.GenerateCacheKey(request);
        var cached = await _redis.GetAsync(key);
        if (cached != null)
            return Ok(cached);

        var results = await _gds.SearchFlightsAsync(request);
        var envelope = new FlightSearchResultEnvolope.FlightSearchResultEnvelope
        {
            Request = request,
            Results = results,
            RetrievedAt = DateTime.UtcNow
        };

        await _redis.SetAsync(key, envelope);
        await _logStorage.AppendLogAsync(key, envelope);

        return Ok(envelope);
    }
}
