using Microsoft.EntityFrameworkCore;
using RouteRunnerLibrary.Models;

namespace RouteRunnerLibrary.Services;

public class SavedRequestService
{
	private readonly AppDbContext _appDbContext;

	public SavedRequestService(AppDbContext appDbContext)
	{
		_appDbContext = appDbContext;
	}
	public Request CreateRequest(Request request)
	{
		_appDbContext.Requests.Add(request);
		_appDbContext.SaveChanges();
		return request;
	}

	public Request GetRequestById(int id)
	{
		return _appDbContext.Requests
			.Include(r => r.Folder)
			.FirstOrDefault(r => r.Id == id);
	}

	public List<Request> GetRequestsByFolderId(int folderId)
	{
		return _appDbContext.Requests
			.Where(r => r.FolderId == folderId)
			.ToList();
	}

	public Request UpdateRequest(Request request)
	{
		if (!_appDbContext.Requests.Any(r => r.Id == request.Id))
		{
			return null; // or throw exception
		}

		_appDbContext.Requests.Update(request);
		_appDbContext.SaveChanges();
		return request;
	}

	public bool DeleteRequest(int id)
	{
		var request = _appDbContext.Requests.Find(id);
		if (request == null)
		{
			return false;
		}

		_appDbContext.Requests.Remove(request);
		_appDbContext.SaveChanges();
		return true;
	}
}
