namespace RouteRunnerLibrary.Models;

public class Folder
{
	public int Id { get; set; }
	public string Name { get; set; }

	public int? ParentId { get; set; }
	public Folder? Parent { get; set; }

	public ICollection<Folder>? SubFolders { get; set; }
	public ICollection<Request>? SavedRequests { get; set; }
}
