using System;

namespace Succubus.Core.Interfaces 
{
	public interface IQueueTransport
	{
		void QueuePublish(object message, string address, Action<Action> marshal = null);
	}
}