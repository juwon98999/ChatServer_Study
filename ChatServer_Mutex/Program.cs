using System;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;


namespace ChatServer_Mutex
{

    public class MyTcpClient
    {

        bool isConnect = false;
        static string server = "127.0.0.1";
        static Int32 port = 9000;
        
        bool ConnectTry = true;


        static void ClientStart()
        {
            NetworkStream Client_stream;
            TcpClient client = new TcpClient();
            Byte[] data = new Byte[512];
            String message;
            MyTcpClient Mc = new MyTcpClient();
            LinkedList<String> Client_list = new LinkedList<String>();
            Mutex Client_mut = new Mutex();
            bool Trigger = false;
            String Read_Message = null;
            bool Client_Write = false;
            bool Server_Writing = false;


            while (Mc.ConnectTry)
            {
                String IpInput = Console.ReadLine();
                //if(IpInput == $"/c {server}:{port}")
                if (IpInput == $"/c")
                {
                    Client_list.AddLast($"{server}:{port}에 접속 시도중...");
                    Client_list.AddLast("'수'님이 접속하셨습니다.");

                    foreach (var chat in Client_list)
                    {
                        Console.WriteLine(chat);
                    }
                    Mc.ConnectTry = false;
                }
                Console.Clear();
            }



            try
            {

                client = new TcpClient(server, port);
                Client_stream = client.GetStream();

                Mc.isConnect = true;

                while (true)
                {

                    Console.Clear();
                    foreach (var chat in Client_list)
                    {
                        Console.WriteLine(chat);
                    }

                        (new Thread(new ThreadStart(() =>
                        {
                            while (Trigger)
                            {
                                //Client_mut.WaitOne();
                                if (Mc.isConnect)
                                {
                                    data = new Byte[512];
                                    Read_Message = null;
                                    Int32 bytes = Client_stream.Read(data, 0, data.Length);
                                    Read_Message = System.Text.Encoding.Default.GetString(data, 0, bytes);


                                    if (Client_list.Count < 10)
                                    {
                                        Client_list.AddLast($"주[] {Read_Message}");

                                        Console.Clear();
                                        foreach (var chat in Client_list)
                                        {
                                            Console.WriteLine(chat);
                                        }
                                        Trigger = false;
                                        Server_Writing = true;
                                    }
                                    else if (Client_list.Count >= 10)
                                    {
                                        LinkedListNode<string> node = Client_list.Find("'수'님이 접속하셨습니다.");
                                        Client_list.AddAfter(node, $"주[] {Read_Message}");
                                        Client_list.RemoveLast();

                                        Console.Clear();
                                        foreach (var chat in Client_list)
                                        {
                                            Console.WriteLine(chat);
                                        }
                                        Trigger = false;
                                        Server_Writing = true;
                                    }

                                }
                                //Client_mut.ReleaseMutex();
                            }

                        }))).Start();

                    if (Server_Writing)
                    {
                        Console.SetCursorPosition(0, 12);
                        Console.WriteLine("주> 대화를 입력중입니다.");
                        Server_Writing = false;
                    }

                    Client_Write = true;

                    //(new Thread(new ThreadStart(() =>
                    //{
                    if (Client_Write)
                    {
                        if (Console.ReadKey().Key == ConsoleKey.D1)
                        {
                            Console.SetCursorPosition(0, 15);
                            Console.WriteLine("메세지를 입력해주세요.");

                      
                            message = Console.ReadLine();

                            if (message == "/q")
                            {
                                Client_stream.Close();
                                client.Close();
                                Console.WriteLine("프로그램이 종료 됩니다.");
                                Func();
                            }
                            data = new Byte[512];
                            data = System.Text.Encoding.Default.GetBytes(message);
                            Client_stream.Write(data, 0, data.Length);

                            if (Client_list.Count < 10)
                            {
                                Client_list.AddLast($"수[] {message}");

                                Console.Clear();
                                foreach (var chat in Client_list)
                                {
                                    Console.WriteLine(chat);
                                }
                                Trigger = true;
                            }
                            else if (Client_list.Count >= 10)
                            {
                                LinkedListNode<string> node = Client_list.Find("'수'님이 접속하셨습니다.");
                                Client_list.AddAfter(node, $"수[] {message}");
                                Client_list.RemoveLast();

                                Console.Clear();
                                foreach (var chat in Client_list)
                                {
                                    Console.WriteLine(chat);
                                }
                                Trigger = true;
                            }
                        }
                    }

                    //}))).Start();

                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
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
            Thread Client = new Thread(new ThreadStart(ClientStart));
            Client.Start();
        }
    }
}
