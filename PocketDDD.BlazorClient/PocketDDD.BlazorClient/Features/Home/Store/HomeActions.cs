using PocketDDD.Shared.API.ResponseDTOs;

namespace PocketDDD.BlazorClient.Features.Home.Store;

public record SetCurrentUser(LoginResultDTO Result);

public record LoadDataAction();
public record SetUserLoggedInAction();
public record SetEventMetaDataAction(
    EventDataResponseDTO EventData,
    ICollection<int> SessionBookmarks,
    ICollection<int> SessionFeedbacks);
public record ToggleBookmarkedAction(int SessionId, bool Bookmarked);
public record GiveSessionFeedbackAction(int SessionId);
