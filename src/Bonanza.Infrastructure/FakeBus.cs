using System;
using System.Collections.Generic;
using System.Threading;

namespace Bonanza.Infrastructure
{
	public class FakeBus : ICommandSender, IEventPublisher
	{
		// handlers list stores both events and commands
		// why handlers but nor handler?
		// answer: In theory we should have 1 handler for one cmd, and many handlers for 1 evnt 
		// since current implementation is used to store both cmd and evnts,
		// there is no restriction here 'one type - one handler' - to allow store handlers for events
		// we have such restriction in 'Send' method
		private readonly Dictionary<Type, List<Action<IMessage>>> _routes = new Dictionary<Type, List<Action<IMessage>>>();

		public void RegisterHandler<T>(Action<T> handler) where T : IMessage
		{
			List<Action<IMessage>> handlers;

			// get handlers list for type
			if (!_routes.TryGetValue(typeof(T), out handlers))
			{
				handlers = new List<Action<IMessage>>();
				_routes.Add(typeof(T), handlers);
			}

			handlers.Add((x => handler((T)x)));
		}

		public void Send<T>(T command) where T : Command
		{
			List<Action<IMessage>> handlers;

			if (_routes.TryGetValue(typeof(T), out handlers))
			{
				if (handlers.Count != 1) throw new InvalidOperationException("cannot send command to more than one handler");
				handlers[0](command);
			}
			else
			{
				throw new InvalidOperationException("no handler registered");
			}
		}

		public void Publish<T>(T @event) where T : Event
		{
			List<Action<IMessage>> handlers;

			if (!_routes.TryGetValue(@event.GetType(), out handlers)) return;

			foreach (var handler in handlers)
			{
				//dispatch on thread pool for added awesomeness
				var handler1 = handler;
				ThreadPool.QueueUserWorkItem(x => handler1(@event));
			}
		}
	}
}
