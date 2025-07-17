FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY DemoRedisS3v2/DemoRedisS3v2.csproj ./DemoRedisS3v2/
COPY SharedLibrary/SharedLibrary.csproj ./SharedLibrary/
RUN dotnet restore "DemoRedisS3v2/DemoRedisS3v2.csproj"

COPY DemoRedisS3v2/ ./DemoRedisS3v2/
COPY SharedLibrary/ ./SharedLibrary/

WORKDIR /src/DemoRedisS3v2
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DemoRedisS3v2.dll"]
