using DemoRedisS3v2.Dtos;

namespace LogStackToRedisWorker;

using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;



public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly string _logDir = "logstack-data";
    private readonly string _archiveDir;
    private readonly TimeSpan _interval;
    private readonly IDatabase _redisDb;
    private readonly TimeSpan _ttl;

    public Worker(ILogger<Worker> logger, IConfiguration config)
    {
        _logger = logger;

        _logDir = config["Logstack:Directory"] ?? "logstack-data";
        _archiveDir = config["Logstack:ArchiveDirectory"] ?? "logstack-archive";
        _interval = TimeSpan.FromSeconds(int.Parse(config["WorkerSettings:IntervalSeconds"] ?? "60"));
        _ttl = TimeSpan.FromMinutes(int.Parse(config["Redis:CacheTTLMinutes"] ?? "60"));

        var redis = ConnectionMultiplexer.Connect(config["Redis:Endpoint"]);
        _redisDb = redis.GetDatabase();

        if (!Directory.Exists(_archiveDir))
            Directory.CreateDirectory(_archiveDir);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var files = Directory.GetFiles(_logDir, "*.json");

            foreach (var file in files)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(file, stoppingToken);
                    var envelope = JsonSerializer.Deserialize<FlightSearchResultEnvolope.FlightSearchResultEnvelope>(json);

                    if (envelope == null || envelope.Request == null)
                    {
                        _logger.LogWarning($"Invalid or empty JSON in file: {file}");
                        continue;
                    }

                    var key = GenerateCacheKey(envelope.Request);
                    await _redisDb.StringSetAsync(key, json, _ttl);
                    _logger.LogInformation($"Cached: {key} (TTL: {_ttl.TotalMinutes} min)");

                    var fileName = Path.GetFileName(file);
                    File.Move(file, Path.Combine(_archiveDir, fileName));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing file: {file}");
                }
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private string GenerateCacheKey(FlightSearchRequest r)
    {
        return $"flight:{r.Source}:{r.Destination}:{r.DepartureTime:yyyyMMddHHmm}-{r.ArrivalTime:yyyyMMddHHmm}";
    }
}

