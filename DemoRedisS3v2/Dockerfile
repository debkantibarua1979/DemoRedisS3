FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["FlightSearch.API/FlightSearch.API.csproj", "FlightSearch.API/"]
COPY ["FlightSearch.Models/FlightSearch.Models.csproj", "FlightSearch.Models/"]
RUN dotnet restore "FlightSearch.API/FlightSearch.API.csproj"
COPY . .
WORKDIR "/src/FlightSearch.API"
RUN dotnet build "FlightSearch.API.csproj" -c Release -o /app/build
RUN dotnet publish "FlightSearch.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FlightSearch.API.dll"]
