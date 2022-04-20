using System;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks; 


namespace ChatServer_Mutex
{

    public class MyTcpClient
    {

        bool isConnect = false;
        Byte[] data = new Byte[256];
        static string server = "127.0.0.1";
        Int32 port = 13000;
        TcpClient client = new TcpClient();
        NetworkStream stream;
        String message;

        static void ClientStart()
        {
            MyTcpClient Mc = new MyTcpClient();
            LinkedList<String> Client_list = new LinkedList<String>();
            

            String IpInput = Console.ReadLine();
            if(IpInput == $"/c {server}")
            {
                Client_list.AddLast($"{MyTcpClient.server}에 접속 시도중...");
                Client_list.AddLast("'수'님이 접속하셨습니다.");

                foreach (var chat in Client_list)
                {
                    Console.WriteLine(chat);
                }
            }


            try
            {

                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer
                // connected to the same address as specified by the server, port
                // combination.%

                Mc.client = new TcpClient(server, Mc.port);
                Mc.stream = Mc.client.GetStream();

                while (true)
                {

                    Mc.isConnect = true;

                    Console.Clear();
                    foreach (var chat in Client_list)
                    {
                        Console.WriteLine(chat);
                    }


                    if (Console.ReadKey().Key == ConsoleKey.T)
                    {
                        Console.WriteLine("메세지를 입력해주세요.");

                        Mc.message = Console.ReadLine();

                        if (Mc.message == "/q")
                        {
                            Mc.stream.Close();
                            Mc.client.Close();
                            Console.WriteLine("프로그램이 종료 됩니다.");
                            Func();
                        }

                        Mc.data = System.Text.Encoding.Default.GetBytes(Mc.message);
                        Mc.stream.Write(Mc.data, 0, Mc.data.Length);
                        Client_list.AddLast($"수[] {Mc.message}");
                    }

                    if (Mc.isConnect == false)
                    {
                        String Read_Message = null;
                        Mc.data = new Byte[256];
                        Read_Message = null;
                        // Read the first batch of the TcpServer response bytes.
                        Int32 bytes = Mc.stream.Read(Mc.data, 0, Mc.data.Length);
                        Read_Message = System.Text.Encoding.Default.GetString(Mc.data, 0, bytes);
                        Client_list.AddLast($"주[] {Read_Message}");
                    }

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
