namespace Bonanza.Infrastructure.Abstractions
{
    public static class HashCodeExtensions
    {
        public static int GetStableHashCode(this string value)
        {
            if (value == null)
                return 42;
            unchecked
            {
                var hash = 5381;
                foreach (var c in value)
                {
                    hash = (hash * 33) ^ c;
                }
                return hash;
            }
        }

        public static int GetStableHashCodeIgnoringCase(this string value)
        {
            return value.ToLowerInvariant().GetStableHashCode();
        }
    }
}