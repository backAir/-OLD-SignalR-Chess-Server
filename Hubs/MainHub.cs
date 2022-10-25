using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace SignalRServer.Hubs
{
    public static class V{

        public static Dictionary<string,List<int>> AllClients = new Dictionary<string, List<int>>();
        public static int amountOfGames = 0;
        public static int move = 0;
        public static Dictionary<int,Game> AllGames = new Dictionary<int, Game>();
    }
    public class Game
    {
        public string[] players = new string[2];

    }
    public class MainHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"Connected: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        // public async Task SendPayloadA(string payload)
        // {
        //     var data = JsonSerializer.Deserialize<dynamic>(payload);
        //     string json = JsonSerializer.Serialize(data);
        //     await Clients.All.SendAsync("ReceivePayloadA", json);
        // }

        // public async Task SendPayloadB(string payload)
        // {
        //     var data = JsonSerializer.Deserialize<dynamic>(payload);
        //     string json = JsonSerializer.Serialize(data);
        //     await Clients.All.SendAsync("ReceivePayloadB", json);
        // }

        public async Task CreateGame(string userName){
            V.amountOfGames++;
            Console.WriteLine("Game "+V.amountOfGames+" created");
            V.AllClients[Context.ConnectionId] = new List<int>();
            V.AllGames[V.amountOfGames] = new Game();
            V.AllGames[V.amountOfGames].players[0] = Context.ConnectionId;
            var gameID = V.amountOfGames;
            var gameMode = "default";
            byte color = 0;
            await Clients.Caller.SendAsync("GameCreated",gameID,gameMode,color);
            //await Clients.Caller.SendAsync("GameCreated",amountOfGames,"10x10");
            //await Clients.All.SendAsync("ReceiveMessage",userName,userName+"b");
        }

    }
}
