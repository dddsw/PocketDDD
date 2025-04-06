using Fluxor;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PocketDDD.BlazorClient.Features.Security.Components;
using PocketDDD.BlazorClient.Services;

namespace PocketDDD.BlazorClient.Features.Security.Store;

public class SecurityEffects(
    LocalStorageContext localStorage,
    IPocketDDDApiService pocketDddapi,
    IDialogService dialog,
    NavigationManager navigationManager)
{
    private IDialogReference? _currentDialogReference;

    [EffectMethod]
    public async Task OnCheckUser(CheckUserAction action, IDispatcher dispatcher)
    {
        var user = await localStorage.CurrentUser.GetAsync();

        if (user is not null)
        {
            dispatcher.Dispatch(new SetCurrentUserAction(user));
            return;
        }

        _currentDialogReference ??= await dialog.ShowAsync<Login>("", new DialogOptions { FullScreen = true });
    }

    [EffectMethod]
    public async Task OnLogin(LoginAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await pocketDddapi.Login(action.LoginName);
            ArgumentNullException.ThrowIfNull(result, nameof(result));

            await localStorage.CurrentUser.SetAsync(result);
            await localStorage.EventScore.SetAsync(1);

            dispatcher.Dispatch(new SetLoginSuccessAction(result));
        }
        catch
        {
            dispatcher.Dispatch(new SetLoginFailed());
        }
    }

    [EffectMethod]
    public Task OnLoginSuccess(SetLoginSuccessAction action, IDispatcher dispatcher)
    {
        dispatcher.Dispatch(new SetCurrentUserAction(action.User));

        _currentDialogReference?.Close();
        _currentDialogReference = null;
        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task OnLoginSuccess(SetCurrentUserAction action, IDispatcher dispatcher)
    {
        pocketDddapi.SetUserAuthToken(action.User.Token);
        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task DeleteAllDataAndLogOut(DeleteAllDataAndLogOutAction action, IDispatcher dispatcher)
    {
        await localStorage.DeleteAllDataAsync();
        navigationManager.NavigateTo("/", true);
    }
}