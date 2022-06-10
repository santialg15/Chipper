namespace LogServer.Responses
{
    public class LogResponse
    {
        public int id { get; set; }
        public string user { get; set; }
        public string action { get; set; }
        public string message { get; set; }
        public string send { get; set; }
        public string receive { get; set; }
    }
}