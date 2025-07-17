using DemoRedisS3v2.Services.Implementations;
using DemoRedisS3v2.Services.Interfaces;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://+:80"); 


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration["Redis:Endpoint"] ?? "localhost:6379"));

builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
builder.Services.AddScoped<IGdsService, MockGdsService>();
builder.Services.AddScoped<ILogStorageService, LogStorageService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});



var app = builder.Build();

app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();