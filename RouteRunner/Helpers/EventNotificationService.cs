using RouteRunnerLibrary.Models;

namespace RouteRunner.Helpers;

public class EventNotificationService
{
	// Singleton instance
	private static EventNotificationService _instance;

	// Event to notify subscribers
	public event EventHandler<SavedRequest> NewRequestCreatedEvent;
	public event EventHandler<(int, string)> RequestNameChangedInTextBoxEvent;
	public event EventHandler<SavedRequest> ExistingRequestSavedEvent;

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

	public void NewRequestCreated(SavedRequest createdRequest)
	{
		NewRequestCreatedEvent?.Invoke(this, createdRequest);
	}


	public void RequestNameChanged(int tabIndex, string requestName)
	{
		RequestNameChangedInTextBoxEvent.Invoke(this, (tabIndex, requestName));
	}

	public void ExistingRequestSaved(SavedRequest updatedRequest)
	{
		ExistingRequestSavedEvent.Invoke(this, updatedRequest);
	}
}
