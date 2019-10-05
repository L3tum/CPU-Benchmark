#region using

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

#endregion

namespace Benchmarking.Results
{
	internal static class ResultUploader
	{
		internal static async Task<string> UploadResult(Save save)
		{
			if (string.IsNullOrEmpty(save.UUID))
			{
				save.UUID = "placeholder";
			}

			var client = new HttpClient();

			var response = await client.PostAsync("https://cpu-benchmark-server.herokuapp.com/uploadSave",
				new StringContent(Convert.ToBase64String(ToByteArray(save)))).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				throw new HttpRequestException(response.ReasonPhrase);
			}

			var uuid = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			return uuid;
		}

		private static byte[] ToByteArray(Save save)
		{
			using (var stream = new MemoryStream())
			{
				using (var writer = new BsonDataWriter(stream))
				{
					var serializer = new JsonSerializer();
					serializer.Serialize(writer, save);

					return stream.ToArray();
				}
			}
		}
	}
}