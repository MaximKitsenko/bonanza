using Bonanza.Infrastructure;
using FluentAssertions;

namespace Bonanza.Contracts.ValueObjects.User
{
    public sealed class UserId : AbstractIdentity<long>
    {
        private const long SystemId = long.MaxValue - 1;

        public override long Id { get; protected set; }

        public override string GetTag()
        {
            return "user";
        }

        public static UserId CreateSystemId()
        {
            return new UserId(SystemId);
        }

        public UserId(long id)
        {
            id.Should().BeGreaterThan(0, "Tried to assemble non-existent user");

            this.Id = id;
        }
    }
}
