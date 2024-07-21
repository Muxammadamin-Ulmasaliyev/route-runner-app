namespace RouteRunnerLibrary;

public interface IApiHelper
{
	Task<string> CallApiAsync(string url, bool formatOutput = true, HttpAction action = HttpAction.GET);
	bool IsValidUrl(string url);
}