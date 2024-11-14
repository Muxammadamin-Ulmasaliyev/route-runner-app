namespace RouteRunnerLibrary.Models;

public class RequestInHistory
{
	public int Id { get; set; }
	public int RequestId { get; set; }
	public DateTime Date { get; set; }
	public string Name { get; set; }
	public string Url { get; set; }
	public HttpVerb HttpVerb { get; set; }
	//public string Headers { get; set; } // JSON string
	public string? Body { get; set; }
}

