namespace RouteRunnerLibrary.Models;

public class HttpResponseInfo
{
	public HttpResponseMessage HttpResponseMessage { get; set; }
	public ResponseTime ResponseTime { get; set; }
	public ResponseSize ResponseSize { get; set; }
}
