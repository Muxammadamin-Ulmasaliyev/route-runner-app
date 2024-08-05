using RouteRunnerLibrary.Models;
using System;

namespace RouteRunnerLibrary;

public interface IApiHelper
{
	Task<HttpResponseInfo> CallApiAsync(string url, HttpVerb action, string bodyContent ,bool formatOutput = true);
	bool IsValidUrl(string url);
}