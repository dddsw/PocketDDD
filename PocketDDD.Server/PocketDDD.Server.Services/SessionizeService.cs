using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PocketDDD.Server.DB;
using PocketDDD.Server.Model.DBModel;
using PocketDDD.Server.Model.Sessionize;
using Session = PocketDDD.Server.Model.DBModel.Session;

namespace PocketDDD.Server.Services;

public class SessionizeService
{
    private readonly PocketDDDContext dbContext;
    private readonly HttpClient httpClient;

    public SessionizeService(HttpClient httpClient, PocketDDDContext dbContext, ILogger<SessionizeService> logger)
    {
        Logger = logger;
        this.httpClient = httpClient;
        this.dbContext = dbContext;
        httpClient.BaseAddress = new Uri("https://sessionize.com/api/v2/");
    }

    private ILogger<SessionizeService> Logger { get; }

    public async Task UpdateFromSessionize()
    {
        Logger.LogInformation("Looking for event detail in database");

        var dbEvent = await dbContext.EventDetail.OrderBy(x => x.Id).LastAsync();
        var sessionizeEventId = dbEvent.SessionizeId;

        Logger.LogInformation("About to get data from Sessionize API");

        var sessionizeEvent = await httpClient.GetFromJsonAsync<SessionizeEvent>($"{sessionizeEventId}/view/All");

        if (sessionizeEvent is null)
            throw new ArgumentNullException(nameof(sessionizeEvent));

        Logger.LogInformation("Information retrieved from Sessionize API");
        Logger.LogInformation("Looking for changes to rooms");

        var dbTracks = await dbContext.Tracks.Where(track => track.EventDetail.Id == dbEvent.Id).ToListAsync();
        foreach (var item in sessionizeEvent.rooms)
        {
            var dbTrack = dbTracks.SingleOrDefault(x => x.SessionizeId == item.id);
            if (dbTrack == null)
            {
                dbTrack = new Track
                {
                    SessionizeId = item.id,
                    EventDetail = dbEvent
                };
                dbContext.Tracks.Add(dbTrack);
            }

            dbTrack.RoomName = item.name;
            dbTrack.Name = $"Track {item.sort}";
        }

        if (dbContext.ChangeTracker.HasChanges())
        {
            dbEvent.Version++;
            Logger.LogInformation("Updating db with changes to rooms");
            await dbContext.SaveChangesAsync();
        }
        else
        {
            Logger.LogInformation("No changes to rooms were detected");
        }

        Logger.LogInformation("Looking for changes to time slots and breaks");

        var dbTimeSlots = await dbContext.TimeSlots.Where(timeSlot => timeSlot.EventDetail.Id == dbEvent.Id)
            .ToListAsync();
        var sessionizeTimeSlots = sessionizeEvent.sessions
            .Select(x => (x.startsAt, x.endsAt, x.isServiceSession,
                serviceSessionDetails: x.isServiceSession ? x.title : null))
            .Distinct()
            .ToList();

        foreach (var item in sessionizeTimeSlots)
        {
            var dbTimeSlot = dbTimeSlots.SingleOrDefault(x => x.From == item.startsAt && x.To == item.endsAt);
            if (dbTimeSlot == null)
            {
                dbTimeSlot = new TimeSlot
                {
                    EventDetail = dbEvent,
                    From = item.startsAt,
                    To = item.endsAt
                };
                dbContext.TimeSlots.Add(dbTimeSlot);
            }

            dbTimeSlot.Info = item.isServiceSession ? item.serviceSessionDetails : null;
        }

        if (dbContext.ChangeTracker.HasChanges())
        {
            dbEvent.Version++;
            Logger.LogInformation("Updating db with changes to time slots and breaks");
            await dbContext.SaveChangesAsync();
        }
        else
        {
            Logger.LogInformation("No changes to time slots or breaks were detected");
        }

        Logger.LogInformation("Looking for changes to sessions");

        var dbSessions = await dbContext.Sessions.Where(session => session.EventDetail.Id == dbEvent.Id).ToListAsync();
        var speakers = sessionizeEvent.speakers;
        dbTracks = await dbContext.Tracks.ToListAsync();
        dbTimeSlots = await dbContext.TimeSlots.ToListAsync();

        foreach (var item in sessionizeEvent.sessions)
        {
            if (item.isServiceSession) continue;

            var sessionizeId = int.Parse(item.id);

            var dbSession = dbSessions.SingleOrDefault(x => x.SessionizeId == sessionizeId);
            if (dbSession == null)
            {
                dbSession = new Session
                {
                    SessionizeId = sessionizeId,
                    EventDetail = dbEvent,
                    SpeakerToken = Guid.NewGuid(),
                    ShortDescription = ""
                };
                dbContext.Sessions.Add(dbSession);
            }

            dbSession.Title = item.title;
            dbSession.FullDescription = item.description;
            dbSession.Speaker = GetSpeakers(speakers, item.speakers);
            dbSession.Track = dbTracks.Single(x => x.SessionizeId == item.roomId);
            dbSession.TimeSlot = GetTimeSlot(dbTimeSlots, item.startsAt, item.endsAt);
        }

        if (dbContext.ChangeTracker.HasChanges())
        {
            dbEvent.Version++;
            Logger.LogInformation("Updating db with changes to sessions");
            await dbContext.SaveChangesAsync();
        }
        else
        {
            Logger.LogInformation("No changes to sessions were detected");
        }
    }

    private string GetSpeakers(List<Speaker> speakers, List<string> speakerIds)
    {
        return speakerIds.Aggregate("", (acc, x) => acc + ", " + speakers.Single(s => s.id == x).fullName).Trim(',');
    }

    private TimeSlot GetTimeSlot(List<TimeSlot> timeSlots, DateTime startsAt, DateTime endsAt)
    {
        return timeSlots.Single(x => x.From == startsAt && x.To == endsAt);
    }
}