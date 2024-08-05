namespace RouteRunnerLibrary.Models
{
	public class SavedRequest
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Url { get; set; }
		public HttpVerb HttpVerb { get; set; }
		//public string Headers { get; set; } // JSON string
		//public string Body { get; set; }
		public int? FolderId { get; set; }
		public Folder? Folder { get; set; }
	}
}
