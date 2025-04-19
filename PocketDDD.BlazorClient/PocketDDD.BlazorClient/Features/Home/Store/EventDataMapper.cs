using PocketDDD.Shared.API.ResponseDTOs;
using System.Collections.Immutable;
using System.Globalization;

namespace PocketDDD.BlazorClient.Features.Home.Store;

public static class EventDataMapper
{
    public static IImmutableList<TimeSlot> ToHomeStateModel(this EventDataResponseDTO eventData, ICollection<int> sessionBookmarks) =>
        eventData.TimeSlots.Select(ts => new TimeSlot
        {
            Id = ts.Id,
            From = ts.From.LocalDateTime,
            To = ts.To.LocalDateTime,
            Info = ts.Info,
            Sessions = eventData.Sessions
                                .Where(s => s.TimeSlotId == ts.Id)
                                .Select(s => new Session
                                {
                                    Id = s.Id,
                                    SpeakerName = s.Speaker,
                                    Title = s.Title,
                                    TrackName = eventData.Tracks.Single(tr => tr.Id == s.TrackId).Name,
                                    RoomName = eventData.Tracks.Single(tr => tr.Id == s.TrackId).RoomName,
                                    IsBookmarked = sessionBookmarks.Contains(s.Id),
                                    Length = ts.To.Subtract(ts.From)
                                })
                                .OrderBy(s => s.TrackName)
                                .ToImmutableList()
        })
        .OrderBy(ts => ts.From)
        .ToImmutableList();
}
