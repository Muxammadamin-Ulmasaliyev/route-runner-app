﻿using RouteRunnerLibrary.Models;

namespace RouteRunner.Helpers;

public class EventNotificationService
{
	// Singleton instance
	private static EventNotificationService _instance;

	// Event to notify subscribers
	public event EventHandler<(int, Request)> NewRequestCreatedEvent;
	public event EventHandler<(int, string)> RequestNameChangedInTextBoxEvent;
	public event EventHandler<Request> ExistingRequestSavedEvent;
	public event EventHandler<Request> RequestDeletedEvent;

	// Private constructor to prevent instantiation
	private EventNotificationService() { }

	// Public method to get the singleton instance
	public static EventNotificationService Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new EventNotificationService();
			}
			return _instance;
		}
	}


	// Method to trigger the event

	public void NewRequestCreated(Request createdRequest, int tabIndex)
	{
		NewRequestCreatedEvent?.Invoke(this, (tabIndex,createdRequest));
	}


	public void RequestNameChanged(int tabIndex, string requestName)
	{
		RequestNameChangedInTextBoxEvent.Invoke(this, (tabIndex, requestName));
	}

	public void ExistingRequestSaved(Request updatedRequest)
	{
		ExistingRequestSavedEvent.Invoke(this, updatedRequest);
	}

	public void RequestDeleted_CollectionsTreeView(Request request)
	{
		RequestDeletedEvent.Invoke(this, request);
	}
}
