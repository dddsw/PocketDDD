using Fluxor;
using MudBlazor;
using PocketDDD.BlazorClient.Services;

namespace PocketDDD.BlazorClient.Features.Home.Store;

public class HomeEffects
{
    private readonly IDialogService _dialog;
    private readonly LocalStorageContext _localStorage;
    private readonly IPocketDDDApiService _pocketDDDAPI;
    private readonly IState<HomeState> _state;

    public HomeEffects(IState<HomeState> state, IDispatcher dispatcher, LocalStorageContext localStorage,
        IPocketDDDApiService pocketDDDAPI, IDialogService dialog)
    {
        _state = state;
        _localStorage = localStorage;
        _pocketDDDAPI = pocketDDDAPI;
        _dialog = dialog;

        _localStorage.EventData.SubscribeToChanges(
            _ => dispatcher.Dispatch(new LoadDataAction()));
        _localStorage.SessionBookmarks.SubscribeToChanges(
            _ => dispatcher.Dispatch(new LoadDataAction()));
        _localStorage.SessionFeedbacks.SubscribeToChanges(
            _ => dispatcher.Dispatch(new LoadDataAction()));
    }

    [EffectMethod]
    public async Task OnLoadData(LoadDataAction action, IDispatcher dispatcher)
    {
        var eventData = await _localStorage.EventData.GetAsync();
        var sessionBookmarks = await _localStorage.SessionBookmarks.GetOrDefaultAsync();
        var sessionFeedbacks = (await _localStorage.SessionFeedbacks.GetOrDefaultAsync()).Select(s => s.SessionId).ToArray();

        if (eventData is not null)
            dispatcher.Dispatch(new SetEventMetaDataAction(eventData, sessionBookmarks, sessionFeedbacks));
    }

    [EffectMethod]
    public async Task OnToggleSessionBookmarked(ToggleBookmarkedAction action, IDispatcher dispatcher)
    {
        var bookmarks = await _localStorage.SessionBookmarks.GetOrDefaultAsync();

        if (action.Bookmarked && !bookmarks.Contains(action.SessionId))
            bookmarks.Add(action.SessionId);
        else if (!action.Bookmarked && bookmarks.Contains(action.SessionId))
            bookmarks.Remove(action.SessionId);

        await _localStorage.SessionBookmarks.SetAsync(bookmarks);
    }
}
