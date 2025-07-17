namespace DemoRedisS3v2.Dtos;

public class FlightSearchRequest
{
    public string Source { get; set; }
    public string Destination { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
}
