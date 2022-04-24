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
            NetworkStream Client_stream2;
            TcpClient client = new TcpClient();
            TcpClient client2 = new TcpClient();
            Byte[] data = new Byte[512];
            String message;
            MyTcpClient Mc = new MyTcpClient();
            LinkedList<String> Client_list = new LinkedList<String>();
            Mutex Client_mut = new Mutex();
            bool Trigger = true;
            String Read_Message = null;
            bool Client_Write = false;
            bool Client_Keyinput = false;


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

                client2 = new TcpClient(server, port);
                Client_stream2 = client2.GetStream();

                Mc.isConnect = true;

                while (true)
                {

                    Console.Clear();
                    foreach (var chat in Client_list)
                    {
                        Console.WriteLine(chat);
                    }


                    //(new Thread(new ThreadStart(() =>
                    //{
                    if (Client_Write)
                    {
                        if (Console.ReadKey().Key == ConsoleKey.D1)
                        {
                            Client_Keyinput = true;
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

                            data = System.Text.Encoding.Default.GetBytes(message);
                            Client_stream.Write(data, 0, data.Length);

                            if (Client_list.Count >= 10)
                            {
                                LinkedListNode<string> node = Client_list.Find("'수'님이 접속하셨습니다.");
                                Client_list.AddAfter(node, $"수[] {message}");
                                Client_list.RemoveLast();

                                Console.Clear();
                                foreach (var chat in Client_list)
                                {
                                    Console.WriteLine(chat);
                                }
                            }
                            else if (Client_list.Count < 10)
                            {
                                Client_list.AddLast($"수[] {message}");

                                Console.Clear();
                                foreach (var chat in Client_list)
                                {
                                    Console.WriteLine(chat);
                                }
                            }
                        }
                
                      
                     }

                //}))).Start();



                (new Thread(new ThreadStart(() =>
                    {
                        while (Trigger)
                        {
                            
                            if (Mc.isConnect)
                            {
                                data = new Byte[512];
                                Read_Message = null;
                                Int32 bytes = Client_stream2.Read(data, 0, data.Length);
                                Read_Message = System.Text.Encoding.Default.GetString(data, 0, bytes);

                                //Client_mut.WaitOne();
                                if (Client_list.Count >= 10)
                                {
                                    LinkedListNode<string> node = Client_list.Find("'수'님이 접속하셨습니다.");
                                    Client_list.AddAfter(node, $"주[] {Read_Message}");
                                    Client_list.RemoveLast();

                                    Console.Clear();
                                    foreach (var chat in Client_list)
                                    {
                                        Console.WriteLine(chat);
                                    }
                                }
                                else if (Client_list.Count < 10)
                                {
                                    Client_list.AddLast($"주[] {Read_Message}");

                                    Console.Clear();
                                    foreach (var chat in Client_list)
                                    {
                                        Console.WriteLine(chat);
                                    }
                                }
                                //Client_mut.ReleaseMutex();
                            }
                        }

                }))).Start();
                Client_Write = true;
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
