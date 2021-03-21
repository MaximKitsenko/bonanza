using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bonanza.Api.IntegrationTests.Utility
{
	public static class HttpContentExtensions
	{
		public static async Task<StringContent> CreateStringContentAsync(this object payload)
		{
			var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(payload));
			var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
			return httpContent;
		}


		public static async Task<StringContent> CreateFromBodyAsync(object payload)
		{
			return await payload.CreateStringContentAsync();
		}
	}
}
