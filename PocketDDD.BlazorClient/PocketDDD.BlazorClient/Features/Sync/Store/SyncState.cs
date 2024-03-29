﻿using Fluxor;
using PocketDDD.Shared.API.ResponseDTOs;

namespace PocketDDD.BlazorClient.Features.Sync.Store;

[FeatureState]
public record SyncState
{
    public LoginResultDTO LoggedInUser { get; init; } = default!;
    public int EventScore { get; set; }
    
    public int OutstandingEventFeedbackSyncCount { get; init; } = 0;
    public int OutstandingSessionFeedbackSyncCount { get; init; } = 0;
    public int OutstandingSyncItems => 
        OutstandingEventFeedbackSyncCount + OutstandingSessionFeedbackSyncCount;

    public bool IsSyncingEvent { get; init; } = false;
    public bool IsSyncingEventFeedback { get; init; } = false;
    public bool IsSyncingSessionFeedback { get; init; } = false;

    public  bool IsSyncing => IsSyncingEvent || IsSyncingEventFeedback || IsSyncingSessionFeedback;
}