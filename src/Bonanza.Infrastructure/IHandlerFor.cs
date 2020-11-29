namespace Bonanza.Infrastructure
{
	public interface IHandlerFor<T>
	{
		void Handle(T message);
	}
}