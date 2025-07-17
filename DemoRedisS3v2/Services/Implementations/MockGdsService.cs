using DemoRedisS3v2.Dtos;
using DemoRedisS3v2.Services.Interfaces;

namespace DemoRedisS3v2.Services.Implementations;


public class MockGdsService : IGdsService
{
    public Task<List<FlightSearchResponse>> SearchFlightsAsync(FlightSearchRequest request)
    {
        var random = new Random();
        var results = new List<FlightSearchResponse>();

        for (int i = 0; i < 3; i++)
        {
            results.Add(new FlightSearchResponse
            {
                FlightNumber = $"XY{random.Next(100, 999)}",
                Airline = "MockAir",
                Source = request.Source,
                Destination = request.Destination,
                DepartureTime = request.DepartureTime.AddHours(i),
                ArrivalTime = request.ArrivalTime.AddHours(i),
                Price = 100 + random.Next(100)
            });
        }

        return Task.FromResult(results);
    }
}
