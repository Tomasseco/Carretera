using System;
using System.Net.Sockets;
using NetworkStreamNS;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string servidorIP = "127.0.0.1";
            int puerto = 5000;

            TcpClient cliente = new TcpClient();
            cliente.Connect(servidorIP, puerto);
            Console.WriteLine("Conectado al servidor");

            NetworkStream stream = cliente.GetStream();
            Console.WriteLine("[Cliente] NetworkStream obtenido");

            // Handshake
            NetworkStreamClass.EscribirMensajeNetworkStream(stream, "INICIO");
            Console.WriteLine("[Cliente] Enviado: INICIO");

            string idRecibido = NetworkStreamClass.LeerMensajeNetworkStream(stream);
            Console.WriteLine($"[Cliente] ID recibido del servidor: {idRecibido}");

            NetworkStreamClass.EscribirMensajeNetworkStream(stream, idRecibido);
            Console.WriteLine("[Cliente] Confirmación de ID enviada");

            cliente.Close();
        }
    }
}
