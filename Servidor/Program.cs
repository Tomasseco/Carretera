using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Servidor
{
    class Program
    {
        static void Main(string[] args)
        {
            int puerto = 5000;
            TcpListener listener = new TcpListener(IPAddress.Any, puerto);

            listener.Start();
            Console.WriteLine("Servidor escuchando en puerto " + puerto);

            while (true)
            {
                TcpClient nuevoCliente = listener.AcceptTcpClient();
                Console.WriteLine("Nuevo cliente conectado desde " + nuevoCliente.Client.RemoteEndPoint);

                // Crea un hilo para gestionar este cliente
                Thread hiloCliente = new Thread(() => GestionarCliente(nuevoCliente));
                hiloCliente.Start();
            }
        }

        static void GestionarCliente(TcpClient cliente)
        {
            Console.WriteLine("Gestionando nuevo vehículo...");

            // Aquí más adelante se gestionará la lógica del cliente

            cliente.Close(); 
            Console.WriteLine("Cliente desconectado");
        }
    }
}
