using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;


namespace Chapter02_Server
{
    class MyTcpListener
    {



 

        static void ServerStart()
        {

            Byte[] bytes = new Byte[512];
            TcpListener server = null;
            bool ClientConnect = false;
            bool Server_Write = false;
            bool Server_Keyinput = false;
            bool Trigger = true;

            String Send_message;
            String Read_data = null;


            NetworkStream Server_stream;
            NetworkStream Server_stream2;
            TcpClient client = new TcpClient();
            TcpClient client2 = new TcpClient();

            LinkedList<String> Server_list = new LinkedList<String>();

            MyTcpListener ML = new MyTcpListener();

            Mutex Server_mut = new Mutex();
            Server_list.AddLast("Wainting Connection...");

            foreach (var chat in Server_list)
            {
                Console.WriteLine(chat);
            }

            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 9000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                ClientConnect = true;
                server = new TcpListener(localAddr, port);
                server.Start();


                Server_list.AddLast("'주' 님께서 연결되었습니다.");
                client = server.AcceptTcpClient();
                client2 = server.AcceptTcpClient();

                Server_stream = client.GetStream();
                Server_stream2 = client2.GetStream();


                while (true)
                {
  
                    Console.Clear();
                    foreach (var chat in Server_list)
                    {
                        Console.WriteLine(chat);
                    }


                    if (Server_Write)
                    {
                        if (Console.ReadKey().Key == ConsoleKey.D1)
                        {
                            Server_Keyinput = true;
                            Console.SetCursorPosition(0, 15);
                            Console.WriteLine("메세지를 입력해주세요.");

                            Send_message = Console.ReadLine();

                            if (Send_message == "/q")
                            {
                                Server_stream.Close();
                                client.Close();
                                Console.WriteLine("프로그램이 종료 됩니다.");
                                Func();
                            }

                            bytes = System.Text.Encoding.Default.GetBytes(Send_message);
                            Server_stream2.Write(bytes, 0, bytes.Length);

                            if (Server_list.Count >= 10)
                            {
                                LinkedListNode<string> node = Server_list.Find("'주' 님께서 연결되었습니다.");
                                Server_list.AddAfter(node, $"주[] {Send_message}");
                                Server_list.RemoveLast();

                                Console.Clear();
                                foreach (var chat in Server_list)
                                {
                                    Console.WriteLine(chat);
                                }
                            }
                            else if (Server_list.Count < 10)
                            {
                                Server_list.AddLast($"주[] {Send_message}");

                                Console.Clear();
                                foreach (var chat in Server_list)
                                {
                                    Console.WriteLine(chat);
                                }
                            }
                            
                        }
                    }
                 


                    (new Thread(new ThreadStart(() =>
                    {

                        while (Trigger)
                        {

                            //while ((i = Server_stream.Read(bytes, 0, bytes.Length)) != 0)
                            //{
        
                            //}

                            if (ClientConnect)
                            {
                                bytes = new Byte[512];
                                Read_data = null;
                                Int32 byt = Server_stream.Read(bytes, 0, bytes.Length);
                                Read_data = System.Text.Encoding.Default.GetString(bytes, 0, byt);
                                Console.WriteLine();

                                //Server_mut.WaitOne();
                                if (Server_list.Count >= 10)
                                {
                                    LinkedListNode<string> node = Server_list.Find("'주' 님께서 연결되었습니다.");
                                    Server_list.AddAfter(node, $"수[] {Read_data}");
                                    Server_list.RemoveLast();

                                    Console.Clear();
                                    foreach (var chat in Server_list)
                                    {
                                        Console.WriteLine(chat);
                                    }
                                }
                                else if (Server_list.Count < 10)
                                {
                                    Server_list.AddLast($"수[] {Read_data}");

                                    Console.Clear();
                                    foreach (var chat in Server_list)
                                    {
                                        Console.WriteLine(chat);
                                    }
                                }
                                //Server_mut.ReleaseMutex();
                            }
                        }

                    }))).Start();
                    Server_Write = true;
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }


        static void ReadTRUE()
        {
            MyTcpListener ML = new MyTcpListener();

          
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
