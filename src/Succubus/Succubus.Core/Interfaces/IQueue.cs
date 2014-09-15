namespace Succubus.Core.Interfaces {

	public interface IQueue
	{
		// --- WORKLOAD MANAGEMENT ---

	       // Add new work item
	       void Queue<T>(T request, string address = null, Action<Action> marshal = null);

	       // Get new work item
	       IResponseContext Dequeue<T>(Action<T> handler, string address = null, Action<Action> marshal = null);
	}

}