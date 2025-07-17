namespace DemoRedisS3v2.Dtos;

public class FlightSearchResponse
{
    public string FlightNumber { get; set; }
    public string Airline { get; set; }
    public string Source { get; set; }
    public string Destination { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal Price { get; set; }
}
