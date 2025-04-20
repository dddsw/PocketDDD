using System.Collections.Immutable;
using Fluxor;

namespace PocketDDD.BlazorClient.Features.Home.Store;

[FeatureState]
public record HomeState
{
    public bool Loading { get; init; } = true;
    public bool FailedToLoad { get; init; } = false;

    public IImmutableList<TimeSlot> Timeslots { get; init; } = ImmutableList<TimeSlot>.Empty;
}

public record TimeSlot
{
    public DateTimeOffset From { get; init; }
    public DateTimeOffset To { get; init; }
    public TimeSpan Length => To.Subtract(From);
    public string? Info { get; init; } = null;
    public IImmutableList<Room> Rooms { get; init; } = ImmutableList<Room>.Empty;
}

public record Session
{
    public int Id { get; init; }
    public DateTimeOffset From { get; init; }
    public DateTimeOffset To { get; init; }
    public string Title { get; init; } = string.Empty;
    public string SpeakerName { get; init; } = string.Empty;
    public bool IsBookmarked { get; set; } = false;
    public TimeSpan Length => To.Subtract(From);
}

public record Room
{
    public string RoomName { get; init; } = string.Empty;
    public IImmutableList<Session> Sessions { get; init; } = ImmutableList<Session>.Empty;
}
