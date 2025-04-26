using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Servidor
{
    class Program
    {
        static int siguienteId = 1;
        static object lockId = new object();
        static Random random = new Random();

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

                Thread hiloCliente = new Thread(() => GestionarCliente(nuevoCliente));
                hiloCliente.Start();
            }
        }

        static void GestionarCliente(TcpClient cliente)
        {
            int idAsignado;
            string direccion;

            lock (lockId)
            {
                idAsignado = siguienteId++;
            }

            direccion = (random.Next(2) == 0) ? "norte" : "sur";
            Console.WriteLine($"Gestionando nuevo vehículo... ID: {idAsignado}, Dirección: {direccion}");

            try
            {
                // Obteneemosmos el NetworkStream del cliente
                NetworkStream stream = cliente.GetStream();
                Console.WriteLine($"NetworkStream obtenido para ID: {idAsignado}");

            
                stream.Close();
                cliente.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error gestionando cliente: " + ex.Message);
            }
        }
    }
}
