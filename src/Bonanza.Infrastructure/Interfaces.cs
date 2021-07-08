namespace Bonanza.Infrastructure
{
	/// <summary>
	/// <para>Interface for the application service, which can handle multiple commands. 
	/// </para>
	/// <para>Application server will host multiple application services, passing commands to them
	/// via this interface. Additional cross-cutting concerns can be wrapped around as necessary 
	///  (<see cref="LoggingWrapper"/>)</para> 
	/// <para>This is only one option of wiring things together. </para>
	/// </summary>
	public interface IApplicationService
	{
		void Execute(ICommand cmd);
	}
	/// <summary><para>
	/// Interface, which marks our events to provide some strong-typing.
	/// In real-world systems we can have more fine-grained interfaces</para> 
	/// </summary>
	public interface IEvent { }

	/// <summary>
	/// <para>Interface for commands, which we send to the application server.
	/// In real-world systems we can have more fine-grained interfaces</para>
	/// </summary>
	public interface ICommand { }

	/// <summary>
	/// Base class for all identities. It might not seem that useful in this sample,
	/// however becomes really useful in the projects, where you have dozens of aggregate
	/// types mixed with stateless (functional) services
	/// </summary>
	public interface IIdentity { }
}
