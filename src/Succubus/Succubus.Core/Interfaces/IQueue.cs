using System;

namespace Succubus.Core.Interfaces {

	public interface IQueue {
		#region Fan out

	       void Send<T>(T request, string address = null, Action<Action> marshal = null);
	       IResponseContext Receive<T>(Action<T> handler, string address = null, Action<Action> marshal = null);

        	#endregion
	}

}