using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bonanza.Api.Controllers;
using FluentAssertions.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Bonanza.Api.IntegrationTests.Controllers
{
	public class TenantControllerTest
	{
		//private readonly TestServer _server;
		//private readonly HttpClient _client;

		public (TestServer _server, HttpClient _client) TenantControllerTest2()
		{
			// Arrange
			var server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
			var client = server.CreateClient();

			return (server, client);
		}

		[SetUp]
		public async Task Setup()
		{

		}

		[Test]
		public async Task NoTenantsCreated_GetTenantsList_EmptyListReturned()
		{
			// arrange
			var (server, client) = TenantControllerTest2();

			// act
			var response = await client.GetAsync("/Tenant/");

			// assert
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();
			Assert.AreEqual("[]", responseString);
		}

		[Test]
		public async Task TenantCreated_GetTenantsList_TenantNameReturned()
		{
			// arrange
			var (server, client) = TenantControllerTest2();

			// act
			var payload = new CreateTenantRequest() {Name = "TestTenant1Name"};
			var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(payload));
			var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
			var createTenantResponse = await client.PostAsync("/Tenant/Create", httpContent);

			// assert
			var response = await client.GetAsync("/Tenant/");
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();

			var expectedResponse = @"[
			{
				""id"": 1,
				""name"": ""TestTenant1Name""
			}
			]";

			var expected = JToken.Parse(expectedResponse);
			var actual = JToken.Parse(responseString);
			actual.Should().BeEquivalentTo(expected);
		}
	}
}
