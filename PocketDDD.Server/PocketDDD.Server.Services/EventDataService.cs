using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PocketDDD.Server.DB;
using PocketDDD.Shared.API.RequestDTOs;
using PocketDDD.Shared.API.ResponseDTOs;

namespace PocketDDD.Server.Services;

public class EventDataService(PocketDDDContext dbContext, IMemoryCache memoryCache, ILogger<EventDataService> logger)
{
    private const string FetchLatestEventDataCacheKey = nameof(FetchLatestEventData);
    private const string FetchCurrentEventDetailIdCacheKey = nameof(FetchCurrentEventDetailId);

    public async Task<int> FetchCurrentEventDetailId()
    {
        if (memoryCache.TryGetValue<int?>(FetchCurrentEventDetailIdCacheKey, out var cachedEventDetailId))
            return cachedEventDetailId!.Value;

        cachedEventDetailId = await dbContext.EventDetail.MaxAsync(e => e.Id);
        memoryCache.Set(FetchCurrentEventDetailIdCacheKey, cachedEventDetailId, TimeSpan.FromMinutes(5));

        return cachedEventDetailId.Value;
    }

    public async Task<EventDataResponseDTO?> FetchLatestEventData(EventDataUpdateRequestDTO requestDto)
    {
        var currentEventDetailId = await FetchCurrentEventDetailId();
        
        if (memoryCache.TryGetValue<EventDataResponseDTO?>(FetchLatestEventDataCacheKey, out var latestEventData))
        {
            logger.LogDebug("Retrieved latest event data from the cache {eventData}", latestEventData);
        }
        else
        {
            logger.LogDebug("No event data in the cache, retrieving from the db");
            latestEventData = await dbContext.EventDetail
                .AsNoTracking()
                .AsSingleQuery()
                .Select(eventDetails => new EventDataResponseDTO
                {
                    Id = eventDetails.Id,
                    Version = eventDetails.Version,
                    TimeSlots = eventDetails.TimeSlots.Select(ts => new TimeSlotDTO
                    {
                        Id = ts.Id,
                        Info = ts.Info,
                        From = ts.From,
                        To = ts.To
                    }).ToList(),
                    Tracks = eventDetails.Tracks.Select(t => new TrackDTO
                    {
                        Id = t.Id,
                        Name = t.Name,
                        RoomName = t.RoomName
                    }).ToList(),
                    Sessions = eventDetails.Sessions.Select(s => new SessionDTO
                    {
                        Id = s.Id,
                        Title = s.Title,
                        ShortDescription = s.ShortDescription,
                        FullDescription = s.FullDescription,
                        Speaker = s.Speaker,
                        TimeSlotId = s.TimeSlot.Id,
                        TrackId = s.Track.Id
                    }).ToList()
                })
                .SingleAsync(e => e.Id == currentEventDetailId);

            memoryCache.Set(FetchLatestEventDataCacheKey, latestEventData, TimeSpan.FromMinutes(5));
            logger.LogDebug("Updated the latest event data in the cache {eventData}", latestEventData);
        }

        if (requestDto.Version >= latestEventData!.Version)
            return null;

        return latestEventData;
    }
}
