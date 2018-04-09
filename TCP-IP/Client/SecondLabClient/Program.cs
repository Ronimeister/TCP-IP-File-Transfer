using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Program
    {
        // Data buffer for incoming data.  
        static byte[] bytes = new byte[1024];

        // Establish the remote endpoint for the socket.  
        // This example uses port 11000 on the local computer.  
        static IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        static IPAddress ipAddress = ipHostInfo.AddressList[0];
        static IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

        // Create a TCP/IP  socket.  
        static Socket sender = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        public static void SaveFile(byte[] bytes, Socket sender)
        {
            int bytesRec = sender.Receive(bytes);

            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(@"D:\file1.dat", FileMode.OpenOrCreate)))
                {
                    writer.Write(Encoding.ASCII.GetString(bytes, 0, bytesRec));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static byte[] ReadFile(string path)
        {
            List<byte> msg = new List<byte>();

            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    // пока не достигнут конец файла
                    // считываем каждое значение из файла
                    while (reader.PeekChar() > -1)
                    {
                        msg.Add(reader.ReadByte());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }          

            return msg.ToArray();
        }

        static void Start()
        {
            Console.WriteLine("Choose the action:");
            Console.WriteLine("1. Send file");
            Console.WriteLine("2. Save file");
            int action = Convert.ToInt32(Console.ReadLine());
            switch (action)
            {
                case 1:
                    {
                        SendFile();
                        break;
                    }
                case 2:
                    {
                        TakeFile();
                        break;
                    }
            }
        }

        public static void SendFile()
        {
            try
            {
                sender.Connect(remoteEP);

                Console.WriteLine("Socket connected to {0}",
                    sender.RemoteEndPoint.ToString());

                // Encode the data string into a byte array.
                byte[] msg = ReadFile(@"C:\Users\Алексей\Desktop\КСИС\file.dat");

                // Send the data through the socket.  
                int bytesSent = sender.Send(msg);

                // Release the socket.  
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }

        }

        public static void TakeFile()
        {
            try
            {
                sender.Connect(remoteEP);

                Console.WriteLine("Socket connected to {0}",
                    sender.RemoteEndPoint.ToString());

                // Encode the data string into a byte array.
                byte[] msg = ReadFile(@"C:\Users\Алексей\Desktop\КСИС\save.dat");

                // Send the data through the socket.  
                int bytesSent = sender.Send(msg);

                // Receive the response from the remote device.
                SaveFile(bytes, sender);

                // Release the socket.  
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
        }

        public static int Main(String[] args)
        {
            Start();
            return 0;
        }
    }
}
