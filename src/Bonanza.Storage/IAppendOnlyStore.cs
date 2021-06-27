using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Bonanza.Storage
{
	public interface IAppendOnlyStore : IDisposable
	{
		/// <summary>
		/// FROM LIDDD:
		/// Appends data to the stream with the specified name.
		/// If <paramref name="expectedVersion"/> is supplied
		/// and it does not match server version, then
		/// <see cref="AppendOnlyStoreConcurrencyException"/>
		/// is thrown.
		/// </summary>
		/// <param name="name">The name of the stream, to 
		/// which data is appended</param>
		/// <param name="data">The data to append</param>
		/// <param name="expectedVersion">The server version 
		/// (supply -1 to append without check).</param>
		/// <exception cref="AppendOnlyStoreConcurrencyException">
		/// thrown when expected server version is supplied and doesn't 
		/// match to server version </exception>
		void Append(
			string name,
			byte[] data,
			long expectedVersion = -1);

		/// <summary>
		/// FROM LIDDD: Read records by stream name.
		/// FROM IDDD: Read Events within a single Stream by 
		/// their names. For rebuilding state of a single Aggregate.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="afterVersion"></param>
		/// <param name="maxCount"></param>
		/// <returns></returns>
		IEnumerable<DataWithVersion> ReadRecords(
			string name,
			long afterVersion,
			int maxCount);

		/// <summary>
		/// From LIDDD: Reads records across all streams.
		/// From IDDD: Read all events in store.
		/// Is Used by infrastructure to replicate Events, to
		/// Publish them without the need for two phase commit,
		/// and to rebuild persistent read models.
		/// </summary>
		/// <param name="afterVersion"></param>
		/// <param name="maxCount"></param>
		/// <returns></returns>
		IEnumerable<DataWithName> ReadRecords(
			int afterVersion,
			int maxCount);

		void Close();

		void Append(string name,
			byte[] data,
			long expectedVersion,
			int tenantId);
	}

	public class DataWithVersion
	{
		public int Version;
		public byte[] Data;

		public DataWithVersion(in int version, byte[] data)
		{
			Version = version;
			Data = data;
		}
	}

	public sealed class DataWithName
	{
		public string Name;
		public byte[] Data;

		public DataWithName(string name, byte[] data)
		{
			Name = name;
			Data = data;
		}
	}

	public enum AppendStrategy
	{
		OnePhase,
		OnePhaseNoVersionCheck,
		TwoPhases
	}

	/// <summary>
	/// Is thrown internally, when storage version does not match the condition 
	/// specified in server request
	/// </summary>
	[Serializable]
    public class AppendOnlyStoreConcurrencyException : Exception
    {
        public long ExpectedStreamVersion { get; private set; }
        public long ActualStreamVersion { get; private set; }
        public string StreamName { get; private set; }

        protected AppendOnlyStoreConcurrencyException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }

        public AppendOnlyStoreConcurrencyException(long expectedVersion, long actualVersion, string name)
            : base(
                string.Format("Expected version {0} in stream '{1}' but got {2}", expectedVersion, name, actualVersion))
        {
            StreamName = name;
            ExpectedStreamVersion = expectedVersion;
            ActualStreamVersion = actualVersion;
        }
    }
}
