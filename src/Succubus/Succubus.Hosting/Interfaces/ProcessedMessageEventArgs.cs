namespace Succubus.Hosting.Interfaces
{
    public class ProcessedMessageEventArgs
    {
        public string Address { get; set;  }
        public object Message { get; set; }
        public string Json { get; set; }
        public string RawJson { get; set; }
    }
}