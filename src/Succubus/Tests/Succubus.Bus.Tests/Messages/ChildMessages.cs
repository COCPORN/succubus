namespace Succubus.Bus.Tests.Messages
{
    public class BaseRequest
    {
        public string Message { get; set; }
    }

    public class ChildRequest : BaseRequest
    {
       
    }

    public abstract class ChildBase
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