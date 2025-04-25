using System;
using System.Net.Sockets;

namespace Client
{
    // Conectando el cliente al servidor
    class Program
    {
        static void Main(string[] args)
        {
            string servidorIP = "127.0.0.1";
            int puerto = 5000;

            TcpClient cliente = new TcpClient();
            cliente.Connect(servidorIP, puerto);

            Console.WriteLine("Conectado al servidor");

            cliente.Close();
        }
    }
}
