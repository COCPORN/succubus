namespace Succubus.Bus.Tests.Messages
{
    public class ChildRequest
    {
        public string Message { get; set; }
    }

    public class ChildBase
    {
        public string Message { get; set; }
    }

    public class ChildResponse1 : ChildBase
    {
       
    }

    public class ChildResponse2 : ChildBase
    {
      
    }
}