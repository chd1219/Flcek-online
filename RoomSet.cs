using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fleck.online
{
    class Room
    {
        public string id { get; set; }
        public List<IWebSocketConnection> listSocket = new List<IWebSocketConnection>();
        public int Count
        {
            get { return listSocket.Count; }
        }

        public IWebSocketConnection GetAt(int index)
        {
            return listSocket.ElementAt(index);
        }

        public void SendAll(string message)
        {
            listSocket.ToList().ForEach(
                                s => s.Send(message)
                                );
        }
    }

    class RoomSet
    {
        List<Room> listRoom = new List<Room>();
        public int Count
        {
            get { return listRoom.Count; }
        }

        public void Add(string id, IWebSocketConnection socket)
        {
            bool isfind = false;
            foreach (Room room in listRoom)
            {
                if (room.id == id)
                {
                    isfind = true;
                    if (!room.listSocket.Contains(socket))
                    {
                        room.listSocket.Add(socket);
                        if (room.Count == 1)
                        {
                            room.SendAll("waiting");
                        }
                        if (room.Count == 2)
                        {
                            room.GetAt(0).Send("you are player 1");
                            room.GetAt(1).Send("you are player 2");
                            room.SendAll("running");
                        }
                        if (room.listSocket.IndexOf(socket) > 2)
                        {
                            socket.Send("the game is started");
                            socket.Send("close");
                        }
                    }
                    break;
                }
            }
            if (!isfind)
            {
                Room newRoom = new Room();
                newRoom.id = id;
                newRoom.listSocket.Add(socket);
                listRoom.Add(newRoom);
            }
        }

        public void Remove(IWebSocketConnection socket)
        {
            foreach (Room r in listRoom)
            {
                if (r.listSocket.Contains(socket))
                {
                    if (r.listSocket.Count > 1)
                    {
                        if (r.GetAt(0) == socket)
                            r.GetAt(1).Send("play 1 is closed");
                        else if (r.GetAt(1) == socket)
                            r.GetAt(0).Send("play 2 is closed");
                    }

                    r.listSocket.Remove(socket);
                    break;
                }
            }
        }

        public void RemoveAll(IWebSocketConnection socket)
        {
            foreach (Room r in listRoom)
            {
                if (r.listSocket.Contains(socket))
                {
                    foreach (IWebSocketConnection sock in r.listSocket)
                    {
                        sock.Close();
                        r.listSocket.Remove(sock);
                    }                   
                    break;
                }
            }
        }

        public void Send(IWebSocketConnection socket, string message)
        {
            foreach (Room r in listRoom)
            {
                if (r.listSocket.Contains(socket))
                {
                    if (r.GetAt(0) == socket)
                        r.GetAt(1).Send(message);
                    else if (r.GetAt(1) == socket)
                        r.GetAt(0).Send(message);
                }
            }
        }
    }        
}
