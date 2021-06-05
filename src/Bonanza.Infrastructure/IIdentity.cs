using System;

namespace Bonanza.Infrastructure
{
	/// <summary>
	/// Strongly-typed identity class. Essentially just an ID with a 
	/// distinct type. It introduces strong-typing and speeds up development
	/// on larger projects. Idea by Jeremie, implementation by Rinat
	/// </summary>
	public interface IIdentity:IComparable
	{
		/// <summary>
		/// Gets the id, converted to a string. Only alphanumerics and '-' are allowed.
		/// </summary>
		/// <returns></returns>
		string GetId();

		/// <summary>
		/// Unique tag (should be unique within the assembly) to distinguish
		/// between different identities, while deserializing.
		/// </summary>
		string GetTag();

	}
}