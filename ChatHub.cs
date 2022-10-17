using Microsoft.AspNetCore.SignalR;



    public static class V{

        public static Dictionary<string,List<int>> AllClients = new Dictionary<string, List<int>>();
        public static int amountOfGames = 0;
        public static int move = 0;
        public static Dictionary<int,Game> AllGames = new Dictionary<int, Game>();
    }
    public class ChatHub : Hub
    {
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
        public async Task JoinGame(string userName, int id){
            Console.WriteLine(userName+" joined game "+id);
            V.AllClients[Context.ConnectionId] = new List<int>();
            V.AllGames[id].players[1] = Context.ConnectionId;
            var gameID = id;
            var gameMode = "default";
            byte color = 1;
            await Clients.Caller.SendAsync("GameJoined",gameID,gameMode,color);
            //await Clients.Caller.SendAsync("GameCreated",amountOfGames,"10x10");
            //await Clients.All.SendAsync("ReceiveMessage",userName,userName+"b");
        }

        public async Task MovePiece(int[] from, ushort[] to, int id){
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
            Console.WriteLine(moveVals+")");

            foreach (var item in V.AllGames[id].players)
            {
                if(!item.Equals(Context.ConnectionId)){
                    await Clients.Client(item).SendAsync("MovePiece",from, to, id);
                }
            }
            //var a = new int[2][]{new int[2]{6,7},new int[]{7,7}};
            //await Clients.Caller.SendAsync("MovePiece",new int[]{6,7}, new ushort[]{3,5}, id);
            //await Clients.Caller.SendAsync("MovePiece",new int[]{6,7}, new ushort[]{3,5}, id);
            V.move++;
        }
        public async Task SendMessage2(Dictionary<int,string> a){
            Console.WriteLine("aaa");
            await Clients.All.SendAsync("ReceiveMessage2",new Dictionary<int,string>(){{2,"lmao"}});
        }
        

    }
    public class Game
    {
        public string[] players = new string[2];

    }
