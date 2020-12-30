using System;
using Bonanza.Contracts.ValueObjects.Tenant;
using Bonanza.Contracts.ValueObjects.User;
using FluentAssertions;

namespace Bonanza.Contracts.ValueObjects
{
	/// <summary>
	/// Needed to identify who sent the command
	/// </summary>
	public class SysInfo : IEquatable<SysInfo>
	{
		public UserId UserId { get; }
		public TenantId TenantId { get; }
		public DateTime SentUtc { get; }

		private SysInfo(DateTime utcTime, UserId userId, TenantId tenantId)
		{
			userId.Should().NotBeNull();
			tenantId.Should().NotBeNull();

			this.UserId = userId;
			this.TenantId = tenantId;
			this.SentUtc = utcTime;
		}

		public static SysInfo CreateSysInfo(TenantId tenantId)
		{
			return CreateSysInfo(tenantId, UserId.CreateSystemId(), DateTime.UtcNow);
		}

		public static SysInfo CreateSysInfo(TenantId tenantId, UserId userId, DateTime dateTime)
		{
			return new SysInfo(dateTime.ToUniversalTime(), userId, tenantId);
		}

		public bool Equals(SysInfo other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(UserId, other.UserId) && Equals(TenantId, other.TenantId) && SentUtc.Equals(other.SentUtc);
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
				var hashCode = UserId.GetHashCode();
				hashCode = (hashCode * 397) ^ TenantId.GetHashCode();
				hashCode = (hashCode * 397) ^ SentUtc.GetHashCode();
				return hashCode;
			}
		}
	}
}
