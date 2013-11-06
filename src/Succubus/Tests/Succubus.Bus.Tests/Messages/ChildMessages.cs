namespace Succubus.Bus.Tests.Messages
{
    public class ChildRequest
    {
        public string Message { get; set; }
    }

    public interface ChildBase
    {
        string Message { get; set; }
    }

    public class ChildResponse1 : ChildBase
    {

        public string Message
        {
            get; set; 
        }
    }

    public class ChildResponse2 : ChildBase
    {

        public string Message
        {
            get; set; 
        }
    }
}