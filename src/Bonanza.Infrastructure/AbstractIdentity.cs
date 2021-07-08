using System;

namespace Bonanza.Infrastructure
{
	public abstract class AbstractIdentity<TKey> : IIdentity
	{
		internal protected const long BlankLongId = long.MaxValue;

		public abstract TKey Id { get; protected set; }

		public string GetId()
		{
			return this.Id.ToString();
		}

		public abstract string GetTag();

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;

			var identity = obj as AbstractIdentity<TKey>;

			if (identity != null)
				return Equals(identity);

			return false;
		}

		public override string ToString()
		{
			return string.Concat(this.GetType().Name.Replace("Id", ""), "-", this.Id);
		}

		public override int GetHashCode()
		{
			// hash code that works across multiple architectures 
			var type = typeof(TKey);
			if (type == typeof(string))
				return this.Id.ToString().GetStableHashCode();
			return this.Id.GetHashCode();
		}

		static AbstractIdentity()
		{
			var type = typeof(TKey);
			if (type == typeof(int) || type == typeof(long) || type == typeof(uint) || type == typeof(ulong))
				return;
			if (type == typeof(Guid) || type == typeof(string))
				return;
			throw new InvalidOperationException("Abstract identity inheritors must provide stable hash. It is not supported for:  " + type);
		}

		public bool Equals(AbstractIdentity<TKey> other)
		{
			if (other != null)
				return other.Id.Equals(this.Id) && other.GetTag() == this.GetTag();

			return false;
		}

		public static bool operator ==(AbstractIdentity<TKey> left, AbstractIdentity<TKey> right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(AbstractIdentity<TKey> left, AbstractIdentity<TKey> right)
		{
			return !Equals(left, right);
		}
	}
}
