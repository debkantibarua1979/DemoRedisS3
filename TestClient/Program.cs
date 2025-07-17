using System.Diagnostics;
using System.Net.Http.Json;
using DemoRedisS3v2.Dtos;
using StackExchange.Redis;

var client = new HttpClient
{
    BaseAddress = new Uri("http://localhost:5050")
};

var redis = await ConnectionMultiplexer.ConnectAsync("localhost:6379");
var redisDb = redis.GetDatabase();

var testCases = new List<FlightSearchRequest>
{
    new()
    {
        Source = "JFK",
        Destination = "LAX",
        DepartureTime = new DateTime(2025, 8, 1, 8, 0, 0, DateTimeKind.Utc),
        ArrivalTime = new DateTime(2025, 8, 1, 12, 0, 0, DateTimeKind.Utc)
    },
    new()
    {
        Source = "DEL",
        Destination = "DXB",
        DepartureTime = new DateTime(2025, 9, 5, 22, 0, 0, DateTimeKind.Utc),
        ArrivalTime = new DateTime(2025, 9, 6, 3, 0, 0, DateTimeKind.Utc)
    },
    new()
    {
        Source = "JFK",
        Destination = "JFK",
        DepartureTime = DateTime.UtcNow,
        ArrivalTime = DateTime.UtcNow.AddHours(2)
    }
};

foreach (var test in testCases)
{
    Console.WriteLine($"\nRequest: {test.Source} ➜ {test.Destination}");

    var sw = Stopwatch.StartNew();
    var response = await client.PostAsJsonAsync("/api/flightsearch", test);
    sw.Stop();

    Console.WriteLine($"⏱️ Response time: {sw.ElapsedMilliseconds} ms");

    if (!response.IsSuccessStatusCode)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Status: {response.StatusCode}");
        Console.ResetColor();
        continue;
    }

    var result = await response.Content.ReadFromJsonAsync<FlightSearchResultEnvolope.FlightSearchResultEnvelope>();

    if (result?.Results == null || !result.Results.Any())
    {
        Console.WriteLine("No results found.");
        continue;
    }

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{result.Results.Count} flights:");
    foreach (var flight in result.Results)
    {
        Console.WriteLine($"{flight.FlightNumber} | {flight.Airline} | {flight.Price} USD");
    }
    Console.ResetColor();

    // Check Redis TTL
    string key = $"flight:{test.Source}:{test.Destination}:{test.DepartureTime:yyyyMMddHHmm}-{test.ArrivalTime:yyyyMMddHHmm}";
    var ttl = await redisDb.KeyTimeToLiveAsync(key);
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"TTL remaining: {(ttl.HasValue ? $"{ttl.Value.TotalMinutes:F1} min" : "none")}");
    Console.ResetColor();

    // Run same request again to validate cache
    Console.WriteLine("Re-checking cached request...");
    var cachedResponse = await client.PostAsJsonAsync("/api/flightsearch", test);
    var fromCache = await cachedResponse.Content.ReadFromJsonAsync<FlightSearchResultEnvolope.FlightSearchResultEnvelope>();
    Console.WriteLine($"Cache returned {fromCache?.Results?.Count ?? 0} flight(s)");
}
