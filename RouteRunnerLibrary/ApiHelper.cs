using RouteRunnerLibrary.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace RouteRunnerLibrary;

public class ApiHelper : IApiHelper
{
	private readonly HttpClient client = new();


	public async Task<HttpResponseInfo> CallApiAsync(
		string url,
		HttpVerb action,
		string bodyContent,
		bool formatOutput = true)
	{

		var bodyStringContent = new StringContent(bodyContent, Encoding.UTF8, "application/json");
		var stopwatch = Stopwatch.StartNew();
		HttpResponseMessage httpResponseMessage;

		switch (action)
		{
			case HttpVerb.GET:
				{
					httpResponseMessage = await client.GetAsync(url);
					break;
				}
			case HttpVerb.POST:
				{
					httpResponseMessage = await client.PostAsync(url, bodyStringContent);
					break;
				}
			case HttpVerb.PUT:
				{
					httpResponseMessage = await client.PutAsync(url, bodyStringContent);
					break;
				}
			case HttpVerb.PATCH:
				{
					httpResponseMessage = await client.PatchAsync(url, bodyStringContent);
					break;
				}
			case HttpVerb.DELETE:
				{
					httpResponseMessage = await client.DeleteAsync(url);

					break;
				}
			case HttpVerb.HEAD:
				{
					httpResponseMessage = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
					break;
				}
			default:
				{
					httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.MethodNotAllowed);
					break;
				}
		}

		stopwatch.Stop();

		ResponseSize responseSize = await GetResponseSize(httpResponseMessage);
		ResponseTime responseTime = await GetResponseTime(stopwatch.Elapsed);

		if (httpResponseMessage.IsSuccessStatusCode)
		{

			string json = await httpResponseMessage.Content.ReadAsStringAsync();
			if (formatOutput)
			{
				var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
				json = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
			}
		}

		return new HttpResponseInfo() { HttpResponseMessage = httpResponseMessage, ResponseTime = responseTime, ResponseSize = responseSize };
	}

	private static async Task<ResponseTime> GetResponseTime(TimeSpan elapsed)
	{
		string metric;
		double amount;
		if (elapsed.TotalMinutes >= 1)
		{
			metric = "m";
			amount = double.Round(elapsed.TotalMinutes, 2);
		}
		else if (elapsed.TotalSeconds >= 1)
		{
			metric = "s";
			amount = double.Round(elapsed.TotalSeconds, 2);
		}
		else if (elapsed.TotalMilliseconds >= 1)
		{
			metric = "ms";
			amount = double.Round(elapsed.TotalMilliseconds, 0);
		}
		else
		{
			metric = "ns";
			amount = double.Round(elapsed.Ticks / 100, 0);

		}
		return new ResponseTime() { Amount = amount, Metric = metric };
	}

	private static async Task<ResponseSize> GetResponseSize(HttpResponseMessage httpResponseMessage)
	{
		double responseSizeAmount;
		string metric = "bytes";

		double headerSize = 0;

		// Calculate the size of the headers
		foreach (var header in httpResponseMessage.Headers)
		{
			headerSize += Encoding.UTF8.GetByteCount(header.Key + ": " + string.Join(", ", header.Value) + "\r\n");
		}

		foreach (var header in httpResponseMessage.Content.Headers)
		{
			headerSize += Encoding.UTF8.GetByteCount(header.Key + ": " + string.Join(", ", header.Value) + "\r\n");
		}

		// Calculate the size of the content
		byte[] contentBytes = await httpResponseMessage.Content.ReadAsByteArrayAsync();
		double contentSize = contentBytes.Length;

		// Total size = headers size + content size
		responseSizeAmount = contentBytes.Length + headerSize;

		if (responseSizeAmount > 1024 * 1024)
		{

			responseSizeAmount = double.Round(responseSizeAmount /= 1024, 2);
			metric = "megabytes";
		}
		if (responseSizeAmount > 1024)
		{
			responseSizeAmount = double.Round(responseSizeAmount /= 1024, 2);
			metric = "kilobytes";
		}


		return new ResponseSize() { Amount = responseSizeAmount, Metric = metric };
	}

	public bool IsValidUrl(string url)
	{
		if (string.IsNullOrWhiteSpace(url))
		{
			return false;
		}

		/*bool output = Uri.TryCreate(url, UriKind.Absolute, out Uri uriOutput) && (uriOutput.Scheme == Uri.UriSchemeHttps);*/

		bool isValid = Uri.TryCreate(url, UriKind.Absolute, out Uri uriOutput);

		return isValid;
	}
}
