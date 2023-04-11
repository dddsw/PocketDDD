﻿using Fluxor;
using Microsoft.AspNetCore.Components;
using PocketDDD.BlazorClient.Features.Home.Store;
using PocketDDD.BlazorClient.Services;

namespace PocketDDD.BlazorClient.Features.Session.Store;

public class SessionEffects
{
    private readonly IState<SessionState> _state;
    private readonly LocalStorageService _localStorage;
    private readonly IPocketDDDApiService _pocketDDDAPI;
    private readonly NavigationManager _navigation;

    public SessionEffects(IState<SessionState> state, LocalStorageService localStorage, IPocketDDDApiService pocketDDDAPI, NavigationManager navigation)
    {
        _state = state;
        _localStorage = localStorage;
        _pocketDDDAPI = pocketDDDAPI;
        _navigation = navigation;
    }

    [EffectMethod]
    public async Task OnViewSession(ViewSessionAction action, IDispatcher dispatcher)
    {
        var eventData = await _localStorage.GetEventData();
        if (eventData is null)
            return;

        var session = eventData.Sessions.Single(x => x.Id == action.SessionId);
        var track = eventData.Tracks.Single(x => x.Id == session.TrackId);
        var timeSlot = eventData.TimeSlots.Single(x => x.Id == session.TimeSlotId);
        dispatcher.Dispatch(new SetSessionAction(session, track, timeSlot));
        _navigation.NavigateTo($"session/{action.SessionId}");
    }

    [EffectMethod]
    public async Task OnToggleSessionBookmarked(ToggleBookmarkedAction action, IDispatcher dispatcher)
    {
        var sessionBookmarks = (await _localStorage.GetSessionBookmarks()) ?? new List<int>();

        if(action.Bookmarked && !sessionBookmarks.Contains(action.SessionId))
            sessionBookmarks.Add(action.SessionId);
        else if(!action.Bookmarked && sessionBookmarks.Contains(action.SessionId))
            sessionBookmarks.Remove(action.SessionId);

        await _localStorage.SetSessionBookmarks(sessionBookmarks);
    }
}