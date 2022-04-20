using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ChatServer_Mutex;
using System.Collections.Generic;


namespace Chapter02_Server
{
    class MyTcpListener
    {

        Byte[] bytes = new Byte[256];
        TcpListener server = null;
        bool ClientConnect = false;
        String Read_data = null;
        NetworkStream stream;
        TcpClient client;

        static void ServerStart()
        {
            MyTcpListener ML = new MyTcpListener();
            LinkedList<String> Server_list = new LinkedList<String>();
            Server_list.AddLast("Wainting Connection...");

            foreach (var chat in Server_list)
            {
                Console.WriteLine(chat);
            }

            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                ML.server = new TcpListener(localAddr, port);
                ML.server.Start();
                ML.ClientConnect = true;


                ML.client = ML.server.AcceptTcpClient();
                Server_list.AddLast("'주' 님께서 연결되었습니다.");


                int i;

                while (true)
                {

                    ML.Read_data = null;
                    ML.stream = ML.client.GetStream();

                    Console.Clear();
                    foreach (var chat in Server_list)
                    {
                        Console.WriteLine(chat);
                    }
 


                    if (Console.ReadKey().Key == ConsoleKey.T)
                    {
                        Console.WriteLine("메세지를 입력해주세요.");
                        String Send_message = Console.ReadLine();
                        if (Send_message == "/q")
                        {
                            ML.stream.Close();
                            ML.client.Close();
                            Console.WriteLine("프로그램이 종료 됩니다.");
                            Func();
                        }

                        Byte[] send_data = System.Text.Encoding.Default.GetBytes(Send_message);
                        ML.stream.Write(send_data, 0, send_data.Length);
                        Console.WriteLine($"주[] {Send_message}");
                    }

                    if (ML.ClientConnect)
                    {
                        if ((i = ML.stream.Read(ML.bytes, 0, ML.bytes.Length)) != 0)
                        {
                            ML.Read_data = System.Text.Encoding.Default.GetString(ML.bytes, 0, i);
                            Server_list.AddLast($"[수] {ML.Read_data}");
                        }
                    }

                    byte[] msg = System.Text.Encoding.Default.GetBytes(ML.Read_data);

                }

            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }

        static void Func()
        {
            Environment.Exit(0);
        }

        public static void Main()
        {
            Thread Server = new Thread(new ThreadStart(ServerStart));
            Server.Start();

        }
    }
}
