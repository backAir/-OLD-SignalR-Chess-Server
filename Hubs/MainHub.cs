using System.Threading;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SignalRServer.Hubs
{
    public static class V{

        public static Dictionary<string,List<int>> AllClients = new Dictionary<string, List<int>>();
        public static int amountOfGames = 0;
        public static Dictionary<int,Game> AllGames = new Dictionary<int, Game>();
        
    }
    public class Game
    {
        public string[] players = new string[2];
        public string gameMode;
        public int moveNumber = 0;
        public int[] lastFrom;
        public int[] lastTo;
        public bool makingMove = false;

    }
    public class MainHub : Hub
    {
        Random random = new Random();
        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"Connected: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }


        public async Task CreateGame(string userName){
            V.amountOfGames++;
            Console.WriteLine("Game "+V.amountOfGames+" created");
            byte color = (byte)random.Next(0,2);
            V.AllClients[Context.ConnectionId] = new List<int>();
            V.AllGames[V.amountOfGames] = new Game();
            V.AllGames[V.amountOfGames].players[color] = Context.ConnectionId;
            var gameID = V.amountOfGames;
            var gameMode = "default";
            await Clients.Caller.SendAsync("GameCreated",gameID,gameMode,color);
            //await Clients.Caller.SendAsync("GameCreated",amountOfGames,"10x10");
            //await Clients.All.SendAsync("ReceiveMessage",userName,userName+"b");
        }
        public async Task JoinGame(string userName, int id){
            for (int i = 0; i < V.AllGames[id].players.Length; i++)
            {
                if(V.AllGames[id].players[i] == Context.ConnectionId){
                    await Clients.Caller.SendAsync("GameJoined",id,V.AllGames[id].gameMode,i);
                    return;
                }else
                {
                    if(V.AllGames[id].players[i]!=null){
                        await Clients.Client(V.AllGames[id].players[i]).SendAsync("AllowMoving",id);   
                    }
                }
            }
            Console.WriteLine(userName+" joined game "+id);
            byte color;
            if(V.AllGames[id].players[0] == null){
                color = 0;
            }else
            {
                color = 1;
            }
            V.AllClients[Context.ConnectionId] = new List<int>();
            V.AllGames[id].players[color] = Context.ConnectionId;
            await Clients.Caller.SendAsync("GameJoined",id,V.AllGames[id].gameMode,color);
            //await Clients.Caller.SendAsync("GameCreated",amountOfGames,"10x10");
            //await Clients.All.SendAsync("ReceiveMessage",userName,userName+"b");
        }

        public async Task SendMove(int[] from, int[] to, int id, int moveNumber){
            if(moveNumber <= V.AllGames[id].moveNumber){
                return;
            }
            V.AllGames[id].makingMove = true;

            PrintMove(from,to,id);
            foreach (var item in V.AllGames[id].players)
            {
                if(!item.Equals(Context.ConnectionId)){
                    await Clients.Client(item).SendAsync("MovePiece",from, to, id,moveNumber);
                }
            }
            //Thread.Sleep(1000);
            
            V.AllGames[id].lastFrom = from;
            V.AllGames[id].lastTo = to;
            V.AllGames[id].moveNumber = moveNumber;
            V.AllGames[id].makingMove = false;
        }
        public async Task checkPacketLoss(int id, int moveNumber)
        {   
            if(moveNumber < V.AllGames[id].moveNumber){
                await Clients.Client(Context.ConnectionId).SendAsync("MovePiece",V.AllGames[id].lastFrom, V.AllGames[id].lastTo, id,moveNumber);
            }else if (moveNumber > V.AllGames[id].moveNumber)
            {
                foreach (var item in V.AllGames[id].players)
                {
                    if(item != Context.ConnectionId)
                        await Clients.Client(Context.ConnectionId).SendAsync("GetForNextMove", id,moveNumber);
                }
            }
        }


        void PrintMove(int[] from, int[] to, int id){
            string moveVals = "Game "+id+": ( ";
            for (int i = 0; i < from.Length; i++)
            {
                moveVals += from[i]+" ";
            }
                moveVals += "-> ";
            foreach (var item in to)
            {
                moveVals += item+" ";   
            }
            Console.WriteLine(moveVals+") "+Context.ConnectionId);
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
    }
}
