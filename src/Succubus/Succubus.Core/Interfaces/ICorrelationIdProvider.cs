namespace Succubus.Core.Interfaces
{
    public interface ICorrelationIdProvider
    {
        string CreateCorrelationId(object o);
    }
}