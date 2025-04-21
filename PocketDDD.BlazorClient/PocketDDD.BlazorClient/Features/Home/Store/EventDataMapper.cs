using System.Collections.Immutable;
using PocketDDD.Shared.API.ResponseDTOs;

namespace PocketDDD.BlazorClient.Features.Home.Store;

public static class EventDataMapper
{
    public static IImmutableList<TimeSlot> ToHomeStateModel(this EventDataResponseDTO eventData,
        ICollection<int> sessionBookmarks)
    {
        // The break timeslots don't have any sessions but still need to be included so starting from eventData.TimeSlots
        var allTimeSlotDtosSortedByTimeAndLength = eventData.TimeSlots.OrderBy(t => t.From).ThenByDescending(t => t.To);

        // Group together the timeslots, rooms and sessions
        Dictionary<TimeSlot, List<(Room room, Session session)>> timeslotRoomSessions =
            allTimeSlotDtosSortedByTimeAndLength.ToDictionary(
                timeSlotDto => new TimeSlot { From = timeSlotDto.From, To = timeSlotDto.To, Info = timeSlotDto.Info },
                timeSlotDto => eventData.Sessions.Where(s => s.TimeSlotId == timeSlotDto.Id)
                    .Select(s =>
                        (
                            new Room
                            {
                                RoomName = eventData.Tracks.Single(track => track.Id == s.TrackId).RoomName
                            },
                            new Session
                            {
                                Id = s.Id,
                                From = timeSlotDto.From,
                                To = timeSlotDto.To,
                                Title = s.Title,
                                SpeakerName = s.Speaker,
                                IsBookmarked = sessionBookmarks.Contains(s.Id)
                            })
                    ).ToList()
            );

        //Deduplicate time slots
        // Sometimes one timeslot may overlap another e.g. 2 x 15 min sessions in one room and 1 x 30 min session in another at the same time
        // We want to take the longer timeslot
        var deduplicatedTimeSlotRoomSessions = new Dictionary<TimeSlot, List<(Room room, Session session)>>();
        foreach (var (timeSlot, roomsAndSessionsInTimeslot) in timeslotRoomSessions)
        {
            var encompassingTimeslot =
                deduplicatedTimeSlotRoomSessions.Keys.SingleOrDefault(k =>
                    k.From <= timeSlot.From && k.To >= timeSlot.To);
            if (encompassingTimeslot is not null)
                deduplicatedTimeSlotRoomSessions[encompassingTimeslot].AddRange(roomsAndSessionsInTimeslot);
            else
                deduplicatedTimeSlotRoomSessions.Add(timeSlot, roomsAndSessionsInTimeslot);
        }

        //And now group by room and fit them into the actual types we want to use
        var homeStateModel = deduplicatedTimeSlotRoomSessions.Select(timeslotRoomSession => timeslotRoomSession.Key with
        {
            Rooms = timeslotRoomSession.Value.GroupBy(tuple => tuple.room, tuple => tuple.session)
                .Select(g => g.Key with
                {
                    Sessions = g.OrderBy(s => s.From).ToImmutableList()
                })
                .OrderBy(r => r.RoomName)
                .ToImmutableList()
        }).ToImmutableList();

        return homeStateModel;
    }
}
