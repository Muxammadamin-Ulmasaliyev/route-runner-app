using Microsoft.EntityFrameworkCore;
using RouteRunnerLibrary.Models;
using System.ComponentModel;
using System.Net;

namespace RouteRunnerLibrary.Services;

public class RequestHistoryService
{
	private readonly AppDbContext _appDbContext;

	public RequestHistoryService(AppDbContext appDbContext)
	{
		_appDbContext = appDbContext;
	}

	public RequestInHistory CreateRequestInHistory(RequestInHistory requestInHistory)
	{
		_appDbContext.RequestsHistory.Add(requestInHistory);
		_appDbContext.SaveChanges();
		return requestInHistory;
	}

	public RequestInHistory GetRequestInHistoryById(int id)
	{
		return _appDbContext.RequestsHistory.FirstOrDefault(r => r.Id == id);
	}

	public List<RequestInHistory> GetRequestsHistory()
	{

		return _appDbContext.RequestsHistory.ToList();
	}

	public List<RequestInHistory> GetRequestHistoryByDay(DateTime date)
	{
		return _appDbContext.RequestsHistory.Where(r => r.Date.Date == date.Date).ToList();
	}

	public async Task<List<DateTime>> GetAvailableDatesFromHistory()
	{
		return await _appDbContext.RequestsHistory
			.AsNoTracking() // Improves performance for read-only operations
			.Select(r => r.Date.Date) // Projects to the date part only
			.Distinct()
			.OrderByDescending(date => date)
			.ToListAsync(); // Executes the query and returns the results as a list
	}

	public async Task<List<IGrouping<DateTime, RequestInHistory>>> GetGroupedRequestsHistoryAsync()
	{
		var groupes = await _appDbContext.RequestsHistory.GroupBy(r => r.Date.Date).ToListAsync();
		return groupes.OrderByDescending(g => g.Key).ToList();
	}
	public bool DeleteRequestInHistory(int id)
	{
		var request = _appDbContext.RequestsHistory.Find(id);
		if (request == null)
		{
			return false;
		}
		_appDbContext.RequestsHistory.Remove(request);
		_appDbContext.SaveChanges();
		return true;
	}

	public void UpsertRequestInHistory(RequestInHistory requestToUpsert)
	{
		var requestInDb = _appDbContext.RequestsHistory.FirstOrDefault(rh => rh.RequestId == requestToUpsert.RequestId);

		if (requestInDb is null)
		{
			CreateRequestInHistory(requestToUpsert);
		}
		else
		{
			requestInDb.Date = DateTime.Now;
			requestInDb.HttpVerb = requestToUpsert.HttpVerb;
			requestInDb.Name = requestToUpsert.Name;
			requestInDb.Url = requestToUpsert.Url;
			requestInDb.RequestId = requestToUpsert.RequestId;
			requestInDb.Body = requestToUpsert.Body;
			_appDbContext.SaveChanges();
		}
	}
}
