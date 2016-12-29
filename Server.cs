using System;
using System.Collections.Generic;
using System.Linq;

namespace Fleck.online
{
    class Server
    {
        static void Main()
        {
            FleckLog.Level = LogLevel.Debug;
            var allSockets = new List<IWebSocketConnection>();
            var server = new WebSocketServer("ws://0.0.0.0:8183");
            RoomSet roomSet = new RoomSet();

            server.Start(socket =>
                {
                    socket.OnOpen = () =>
                        {
                            Console.WriteLine("Open!");
                            allSockets.Add(socket);  
                        };
                    socket.OnClose = () =>
                        {
                            Console.WriteLine(socket.ConnectionInfo.ClientIpAddress + ":"+ socket.ConnectionInfo.ClientPort.ToString() + " Close!");
                            allSockets.Remove(socket);
                            roomSet.Remove(socket);
                        };
                    socket.OnMessage = message =>
                        {
                            if (message.IndexOf("roomid") != -1)
                            {
                                roomSet.Add(message, socket);                                
                            }
                            else if (message.IndexOf("move") != -1)
                            {
                                roomSet.Send(socket, message);
                            }
                            else if (message.IndexOf("resign") != -1)
                            {
                                roomSet.RemoveAll(socket);
                            }                             
                                          
                            Console.WriteLine(message);
                            
                        };
                });
            
            var input = Console.ReadLine();
            while (input != "exit")
            {
                foreach (var socket in allSockets.ToList())
                {
                    socket.Send(input);
                }
                input = Console.ReadLine();
            }
        }
    }  
}
