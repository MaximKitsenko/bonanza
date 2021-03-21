using FluentAssertions.Json;
using Newtonsoft.Json.Linq;

namespace Bonanza.Api.IntegrationTests.Utility
{
	public class Assert
	{
		public static void JsonResponseIsEquivalentTo(string expectedResponse, string actualResponse)
		{
			var expected = JToken.Parse(expectedResponse);
			var actual = JToken.Parse(actualResponse);
			actual.Should().BeEquivalentTo(expected);
		}
	}
}