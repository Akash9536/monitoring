namespace flx_web
{
    public class Alert
    {
        public DateTime Timestamp { get; set; }
        public int Severity { get; set; }
        public string? Customer { get; set; }
        public string? Message { get; set; }
    }
}
