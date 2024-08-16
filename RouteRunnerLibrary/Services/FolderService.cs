using Microsoft.EntityFrameworkCore;
using RouteRunnerLibrary.Models;

namespace RouteRunnerLibrary.Services;

public class FolderService
{
	private readonly AppDbContext _appDbContext;

	public FolderService(AppDbContext appDbContext)
	{
		_appDbContext = appDbContext;
	}



	// Folder CRUD operations
	public Folder CreateFolder(Folder folder)
	{
		_appDbContext.Folders.Add(folder);
		_appDbContext.SaveChanges();
		return folder;
	}

	public Folder GetFolderById(int id)
	{
		return _appDbContext.Folders
			.Include(f => f.SubFolders)
			.Include(f => f.SavedRequests)
			.FirstOrDefault(f => f.Id == id);
	}

	public List<Folder> GetAllFolders()
	{
		return _appDbContext.Folders
			.Include(f => f.SubFolders)
			.Include(f => f.SavedRequests)
			.ToList();
	}

	public Folder UpdateFolder(Folder folder)
	{
		if (!_appDbContext.Folders.Any(f => f.Id == folder.Id))
		{
			return null; // or throw exception
		}

		_appDbContext.Folders.Update(folder);
		_appDbContext.SaveChanges();
		return folder;
	}

	public bool DeleteFolder(int id)
	{
		var folder = _appDbContext.Folders.Find(id);

		if (folder == null)
		{
			return false;
		}

		// Delete subfolders and requests
		var subFolders = _appDbContext.Folders.Where(f => f.ParentId == id).ToList();
		var requests = _appDbContext.SavedRequests.Where(r => r.FolderId == id).ToList();

		_appDbContext.Folders.RemoveRange(subFolders);
		_appDbContext.SavedRequests.RemoveRange(requests);
		_appDbContext.Folders.Remove(folder);

		_appDbContext.SaveChanges();
		return true;
	}

	public List<Folder> GetRootParentFolders()
	{
		return _appDbContext.Folders
			.Include(f => f.SubFolders)
			.Include(f => f.SavedRequests)
			.Where(f => f.ParentId == null)
			.ToList();
	}

	public List<Folder> GetSubFolders(int parentFolderId)
	{
		return _appDbContext.Folders
			.Include(f => f.SubFolders)
			.Include(f => f.SavedRequests)
			.Where(f => f.ParentId == parentFolderId)
			.ToList();	
	}

	// Request CRUD operations

}
