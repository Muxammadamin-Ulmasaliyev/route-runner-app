namespace RouteRunnerLibrary.Models
{
	public class Request
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Url { get; set; }
		public HttpVerb HttpVerb { get; set; }
		public List<Header> Headers { get; set; }
		public string? Body { get; set; }
		public int? FolderId { get; set; }
		public Folder? Folder { get; set; }
	}
}
