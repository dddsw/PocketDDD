using System.Collections.Immutable;
using Fluxor;

namespace PocketDDD.BlazorClient.Features.Home.Store;

[FeatureState]
public record HomeState
{
    public bool Loading { get; init; } = true;
    public bool FailedToLoad { get; init; } = false;

    public IImmutableList<TimeSlot> EventMetaData { get; init; } = ImmutableList<TimeSlot>.Empty;
}

public record TimeSlot
{
    public int Id { get; init; }
    public DateTimeOffset From { get; init; }
    public DateTimeOffset To { get; init; }
    public string? Info { get; init; } = null;
    public IImmutableList<Session> Sessions { get; init; } = ImmutableList<Session>.Empty;
}

public record Session
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string SpeakerName { get; init; } = string.Empty;
    public string TrackName { get; init; } = string.Empty;
    public string RoomName { get; init; } = string.Empty;
    public bool IsBookmarked { get; set; } = false;
    public TimeSpan Length { get; init; }
}
