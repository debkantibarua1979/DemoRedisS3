using LogStackToRedisWorker;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services.AddHostedService<Worker>();
    })
    .Build()
    .Run();