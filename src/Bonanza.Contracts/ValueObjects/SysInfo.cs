using System;

namespace Bonanza.Contracts.ValueObjects
{

	public class SysInfo : IEquatable<SysInfo>
	{
		public UserId UserId { get; private set; }
		public TenantId TenantId { get; private set; }

		public DateTime SentUtc { get; private set; }

		private SysInfo(DateTime utcTime, UserId userId)
		{
			this.UserId = userId;
			this.SentUtc = utcTime;
		}

		public static SysInfo CreateSysInfo()
		{
			return CreateSysInfo(UserId.SystemUserId);
		}

		public static SysInfo CreateSysInfo(UserId userId)
		{
			return CreateSysInfo(userId, DateTime.UtcNow);
		}

		public static SysInfo CreateSysInfo(UserId userId, DateTime dateTime)
		{
			return new SysInfo(dateTime.ToUniversalTime(), userId);
		}

		public bool Equals(SysInfo other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(UserId, other.UserId) && SentUtc.Equals(other.SentUtc);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((SysInfo)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((UserId != null ? UserId.GetHashCode() : 0) * 397) ^ SentUtc.GetHashCode();
			}
		}
	}
}