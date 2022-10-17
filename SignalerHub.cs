using Microsoft.AspNetCore.SignalR;
public class SignalerHub : Hub
{
    public async Task ChangeName(string name){
        await Clients.All.SendAsync("UpdatePlayerName", name);
    }
}