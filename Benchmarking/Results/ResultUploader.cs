#region using

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CPU_Benchmark_Common;
using Newtonsoft.Json;

#endregion

namespace Benchmarking.Results
{
	internal static class ResultUploader
	{
		internal static async Task<UploadedResponse> UploadResult(Save save)
		{
			save.UUID = "placeholder";

			using var client = new HttpClient();

			var response = await client.PostAsync("https://cpu-benchmark-server.herokuapp.com/uploadSave/v2",
				new StringContent(Convert.ToBase64String(ToByteArray(save)))).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				throw new HttpRequestException(response.ReasonPhrase);
			}

			var uploadedResponse =
				JsonConvert.DeserializeObject<UploadedResponse>(response.Content.ReadAsStringAsync().Result);

			return uploadedResponse;
		}

		private static byte[] ToByteArray(Save save)
		{
			return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(save));
		}
	}
}