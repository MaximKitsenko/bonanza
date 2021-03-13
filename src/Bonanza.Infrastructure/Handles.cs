namespace Bonanza.Infrastructure
{
	public interface Handles<T>
	{
		void Handle(T message);
	}
}
