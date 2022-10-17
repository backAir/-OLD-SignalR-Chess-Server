// using Microsoft.AspNetCore.SignalR.Client;

// namespace Client
// {
//     public class Program
//     {
//         private static HubConnection _connection;
//         public static async Task Main(string[] args)
//         {            
//             _connection = new HubConnectionBuilder()
//                 .WithUrl("https://localhost:7116/chatHub")                
//                 .Build();

//             _connection.Closed += async (error) =>
//             {
//                 await Task.Delay(new Random().Next(0, 5) * 1000);
//                 await _connection.StartAsync();
//             };

//             await ConnectAsync();
            
//             bool stop = false;
//             while (!stop)
//             {
//                 Console.WriteLine("Press any key to send message to server and receive echo");
//                 Console.ReadKey();
//                 Send("testuser", "msg");
//                 Console.WriteLine("Press q to quit or anything else to resume");
//                 var key = Console.ReadLine();
//                 if (key == "q") stop = true;
//             }                        
//         }

//         private static async Task ConnectAsync()
//         {
//             _connection.On<string, string>("ReceiveMessage", (user, message) =>
//             {
//                 Console.WriteLine("Received message");
//                 Console.WriteLine($"user: {user}");
//                 Console.WriteLine($"message: {message}");
//             });
//             try
//             {
//                 await _connection.StartAsync();
//             }
//             catch(Exception e)
//             {
//                 Console.WriteLine("Exception: {0}", e);
//             }
//         }

//         private static async void Send(string user, string msg)
//         {
//             try
//             {
//                 await _connection.InvokeAsync("SendMessage", user, msg);
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine($"Exception: {e}");
//             }
//         }
//     }
// }