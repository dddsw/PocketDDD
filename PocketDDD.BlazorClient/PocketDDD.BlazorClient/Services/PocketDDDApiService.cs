using System.Net;
using System.Net.Http.Json;
using PocketDDD.Shared.API.RequestDTOs;
using PocketDDD.Shared.API.ResponseDTOs;

namespace PocketDDD.BlazorClient.Services;

public interface IPocketDDDApiService
{
    Task<LoginResultDTO> Login(string name);
    void SetUserAuthToken(string token);

    Task<EventDataResponseDTO?> FetchLatestEventData(EventDataUpdateRequestDTO requestDTO);
    Task<FeedbackResponseDTO> SubmitClientEventFeedback(SubmitEventFeedbackDTO feedbackDTO);
    Task<FeedbackResponseDTO> SubmitClientSessionFeedback(SubmitSessionFeedbackDTO feedbackDTO);
}

public class PocketDDDApiService : IPocketDDDApiService
{
    private readonly HttpClient _http;

    public PocketDDDApiService(HttpClient http)
    {
        _http = http;
    }

    public void SetUserAuthToken(string token)
    {
        _http.DefaultRequestHeaders.Clear();
        _http.DefaultRequestHeaders.Add("Authorization", token);
    }

    public async Task<EventDataResponseDTO?> FetchLatestEventData(EventDataUpdateRequestDTO requestDTO)
    {
        var response = await _http.PostAsJsonAsync<EventDataUpdateRequestDTO>("EventData/FetchLatestEventData", requestDTO);

        if (response.StatusCode == HttpStatusCode.NoContent)
            return null;

        return (await response.Content.ReadFromJsonAsync<EventDataResponseDTO>())!;
    }
    
    public async Task<LoginResultDTO> Login(string name)
    {
        var dto = new RegisterDTO { Name = name };
        var response = await _http.PostAsJsonAsync("Registration/Login", dto);
        return (await response.Content.ReadFromJsonAsync<LoginResultDTO>())!;
    }

    public async Task<FeedbackResponseDTO> SubmitClientEventFeedback(SubmitEventFeedbackDTO feedbackDTO)
    {
        var response = await _http.PostAsJsonAsync("Feedback/ClientEventFeedback", feedbackDTO);
        return (await response.Content.ReadFromJsonAsync<FeedbackResponseDTO>())!;
    }

    public async Task<FeedbackResponseDTO> SubmitClientSessionFeedback(SubmitSessionFeedbackDTO feedbackDTO)
    {
        var response = await _http.PostAsJsonAsync("Feedback/ClientSessionFeedback", feedbackDTO);
        return (await response.Content.ReadFromJsonAsync<FeedbackResponseDTO>())!;
    }
}