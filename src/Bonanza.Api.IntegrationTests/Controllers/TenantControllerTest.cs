using System.Net.Http;
using System.Threading.Tasks;
using Bonanza.Api.Controllers;
using Bonanza.Api.IntegrationTests.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;
using Assert = Bonanza.Api.IntegrationTests.Utility.Assert;

namespace Bonanza.Api.IntegrationTests.Controllers
{
	public class TenantControllerTest
	{
		public (TestServer _server, HttpClient _client) CreateServerAndClient()
		{
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
			var (server, client) = CreateServerAndClient();
			var expectedResponse = @"[]";

			// act
			var response = await client.GetAsync("/Tenant/");

			// assert
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();
			Assert.JsonResponseIsEquivalentTo(expectedResponse, responseString);
		}

		[Test]
		public async Task TenantCreated_GetTenantsList_TenantNameReturned()
		{
			// arrange
			var (server, client) = CreateServerAndClient();
			var expectedResponse = @"[{	""id"": 1,	""name"": ""TestTenant1Name""}]";

			// act
			var payload = await HttpContentExtensions.CreateFromBodyAsync(new CreateTenantRequest() { Name = "TestTenant1Name" }) ;
			var createTenantResponse = await client.PostAsync("/Tenant/Create", payload);

			// assert
			var response = await client.GetAsync("/Tenant/");
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();

			Assert.JsonResponseIsEquivalentTo(expectedResponse, responseString);
		}

		[Test]
		public async Task MultipleTenantsCreated_GetTenantsList_TenantsNamesReturned()
		{
			// arrange
			var (server, client) = CreateServerAndClient();
			var expectedResponse = @"[
										{	""id"": 1,	""name"": ""TestTenant1Name""},
										{	""id"": 2,	""name"": ""TestTenant2Name""}
									]";

			// act
			var createTenantResponse = await client.PostAsync(
				"/Tenant/Create",
				await HttpContentExtensions.CreateFromBodyAsync(new CreateTenantRequest()
					{
						Name = "TestTenant1Name"
					}));

			var createTenantResponse2 = await client.PostAsync(
				"/Tenant/Create",
				await HttpContentExtensions.CreateFromBodyAsync(new CreateTenantRequest()
					{
						Name = "TestTenant2Name"
					}));

			// assert
			var response = await client.GetAsync("/Tenant/");
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();

			Assert.JsonResponseIsEquivalentTo(expectedResponse, responseString);
		}
	}
}
