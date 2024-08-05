using Microsoft.EntityFrameworkCore;
using RouteRunnerLibrary.Models;

namespace RouteRunnerLibrary.Services;

public class SavedRequestService
{
	private readonly AppDbContext _appDbContext;

	public SavedRequestService(AppDbContext _appDbContext)
	{
		_appDbContext = _appDbContext;
	}
	public SavedRequest CreateRequest(SavedRequest request)
	{
		_appDbContext.SavedRequests.Add(request);
		_appDbContext.SaveChanges();
		return request;
	}

	public SavedRequest GetRequestById(int id)
	{
		return _appDbContext.SavedRequests
			.Include(r => r.Folder)
			.FirstOrDefault(r => r.Id == id);
	}

	public List<SavedRequest> GetRequestsByFolderId(int folderId)
	{
		return _appDbContext.SavedRequests
			.Where(r => r.FolderId == folderId)
			.ToList();
	}

	public SavedRequest UpdateRequest(SavedRequest request)
	{
		if (!_appDbContext.SavedRequests.Any(r => r.Id == request.Id))
		{
			return null; // or throw exception
		}

		_appDbContext.SavedRequests.Update(request);
		_appDbContext.SaveChanges();
		return request;
	}

	public bool DeleteRequest(int id)
	{
		var request = _appDbContext.SavedRequests.Find(id);
		if (request == null)
		{
			return false;
		}

		_appDbContext.SavedRequests.Remove(request);
		_appDbContext.SaveChanges();
		return true;
	}
}
