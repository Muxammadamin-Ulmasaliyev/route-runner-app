using RouteRunnerLibrary.Models;

public class Header
{
	public int Id { get; set; }
    public bool IsEnabled { get; set; }
    public string Key { get; set; }
	public string Value { get; set; }
	public string? Description { get; set; }
	public int RequestId { get; set; }
	public Request Request { get; set; }
}
