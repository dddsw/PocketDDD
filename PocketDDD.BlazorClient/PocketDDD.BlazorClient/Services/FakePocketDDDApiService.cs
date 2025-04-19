using PocketDDD.Shared.API.RequestDTOs;
using PocketDDD.Shared.API.ResponseDTOs;

namespace PocketDDD.BlazorClient.Services;

public class FakePocketDDDApiService : IPocketDDDApiService
{
    public Task<LoginResultDTO> Login(string name)
    {
        return Task.FromResult(new LoginResultDTO(name, Guid.NewGuid().ToString()));
    }

    public void SetUserAuthToken(string token)
    {
    }

    public async Task<EventDataResponseDTO?> FetchLatestEventData(EventDataUpdateRequestDTO requestDTO)
    {
        if (requestDTO.Version == 1)
        {
            await Task.Delay(1000);
            return null;
        }

        return
            new EventDataResponseDTO
            {
                Id = 1,
                Version = 1,
                TimeSlots = new[]
                {
                    new TimeSlotDTO
                    {
                        Id = 1,
                        From = new DateTimeOffset(2023, 4, 29, 8, 00, 0, TimeSpan.Zero),
                        To = new DateTimeOffset(2023, 4, 29, 8, 30, 0, TimeSpan.Zero),
                        Info = "Registration"
                    },
                    new TimeSlotDTO
                    {
                        Id = 2,
                        From = new DateTimeOffset(2023, 4, 29, 8, 30, 0, TimeSpan.Zero),
                        To = new DateTimeOffset(2023, 4, 29, 9, 0, 0, TimeSpan.Zero),
                        Info = "Intro"
                    },
                    new TimeSlotDTO
                    {
                        Id = 3,
                        From = new DateTimeOffset(2023, 4, 29, 9, 00, 0, TimeSpan.Zero),
                        To = new DateTimeOffset(2023, 4, 29, 10, 00, 0, TimeSpan.Zero)
                    },
                    new TimeSlotDTO
                    {
                        Id = 4,
                        From = new DateTimeOffset(2023, 4, 29, 10, 0, 0, TimeSpan.Zero),
                        To = new DateTimeOffset(2023, 4, 29, 10, 20, 0, TimeSpan.Zero),
                        Info = "Coffee"
                    },
                    new TimeSlotDTO
                    {
                        Id = 5,
                        From = new DateTimeOffset(2023, 4, 29, 10, 20, 0, TimeSpan.Zero),
                        To = new DateTimeOffset(2023, 4, 29, 11, 20, 0, TimeSpan.Zero)
                    }
                },
                Tracks = new[]
                {
                    new TrackDTO { Id = 1, Name = "Track 1", RoomName = "Room 1" },
                    new TrackDTO { Id = 2, Name = "Track 2", RoomName = "Room 2" },
                    new TrackDTO { Id = 3, Name = "Track 3", RoomName = "Room 3" }
                },
                Sessions = new[]
                {
                    new SessionDTO
                    {
                        Id = 1,
                        FullDescription = "Some full desk",
                        Speaker = "Ross",
                        TimeSlotId = 3,
                        TrackId = 1,
                        Title = "Blazor Session Management"
                    },
                    new SessionDTO
                    {
                        Id = 2,
                        FullDescription = "Second session",
                        Speaker = "Jim",
                        TimeSlotId = 3,
                        TrackId = 2,
                        Title = "How to code"
                    },
                    new SessionDTO
                    {
                        Id = 3,
                        FullDescription = "Third session",
                        Speaker = "Bob",
                        TimeSlotId = 5,
                        TrackId = 2,
                        Title = "Off by 1"
                    }
                }
            };
    }

    public Task<FeedbackResponseDTO> SubmitClientEventFeedback(SubmitEventFeedbackDTO feedbackDTO)
    {
        return Task.FromResult(new FeedbackResponseDTO { ClientId = feedbackDTO.ClientId, Score = 2 });
    }

    public Task<FeedbackResponseDTO> SubmitClientSessionFeedback(SubmitSessionFeedbackDTO feedbackDTO)
    {
        return Task.FromResult(new FeedbackResponseDTO { ClientId = feedbackDTO.ClientId, Score = 3 });
    }
}