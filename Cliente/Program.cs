using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using NetworkStreamNS;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Conectando al servidor...");
                TcpClient cliente = new TcpClient("127.0.0.1", 5000);
                NetworkStream ns = cliente.GetStream();

                Console.WriteLine("Conectado al servidor. Enviando mensaje de inicio...");

                // Handshake
                NetworkStreamClass.EscribirMensajeNetworkStream(ns, "INICIO");

                string idRecibido = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                Console.WriteLine($"ID recibido del servidor: {idRecibido}");

                // Confirmación
                NetworkStreamClass.EscribirMensajeNetworkStream(ns, idRecibido);

                Console.WriteLine("Handshake completado. Cliente listo.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
