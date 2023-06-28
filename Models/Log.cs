namespace RestaurantOrderingSystem.Models
{
    public class Log
    {
        public int ID { get; set; }
        public string RequestPath { get; set; }
        public string HttpMethod { get; set; }
        public DateTime RequestTime { get; set; }
        public string RequestBody { get; set; }
        public int StatusCode { get; set; }
        public DateTime ResponseTime { get; set; }
        public string ResultBody { get; set; }
    }


}
