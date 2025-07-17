namespace DemoRedisS3v2.Dtos;

public class FlightSearchResultEnvolope
{
    public class FlightSearchResultEnvelope
    {
        public FlightSearchRequest Request { get; set; }
        public List<FlightSearchResponse> Results { get; set; }
        public DateTime RetrievedAt { get; set; }
    }
}