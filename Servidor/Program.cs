// Servidor/Program.cs
using System;
using System.Net;
using System.Net.Sockets;

namespace Servidor
{
    // Servidor TCP que escucha a un un cliente
    class Program
    {
        static void Main(string[] args)
        {
            int puerto = 5000;
            TcpListener listener = new TcpListener(IPAddress.Any, puerto);

            listener.Start();
            Console.WriteLine("Servidor escuchando en puerto " + puerto);

            TcpClient cliente = listener.AcceptTcpClient();
            Console.WriteLine("Cliente conectado desde " + cliente.Client.RemoteEndPoint);

            // Cerrar conexiones para este ejemplo
            cliente.Close();
            listener.Stop();
        }
    }
}
