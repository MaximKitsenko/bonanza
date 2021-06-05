using System;
using System.Runtime.Serialization;

namespace Bonanza.Infrastructure.Abstractions
{
    public interface IIdentity //:IComparable
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
