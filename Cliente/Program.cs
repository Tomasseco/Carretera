using System;
using System.Net.Sockets;
using System.IO;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string servidorIP = "127.0.0.1";
            int puerto = 5000;

            try
            {
                TcpClient cliente = new TcpClient();
                cliente.Connect(servidorIP, puerto);

                Console.WriteLine("Conectado al servidor.");

                // Obtenemos NetworkStream
                NetworkStream stream = cliente.GetStream();
                Console.WriteLine("NetworkStream obtenido.");

  
                stream.Close();
                cliente.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al conectar: " + ex.Message);
            }
        }
    }
}
