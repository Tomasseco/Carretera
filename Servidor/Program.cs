using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetworkStreamNS;

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

            NetworkStream stream = cliente.GetStream();

            // Handshake
            string mensajeInicio = NetworkStreamClass.LeerMensajeNetworkStream(stream);
            Console.WriteLine($"[Servidor] Recibido del cliente: {mensajeInicio}");

            if (mensajeInicio == "INICIO")
            {
                NetworkStreamClass.EscribirMensajeNetworkStream(stream, idAsignado.ToString());
                Console.WriteLine($"[Servidor] Enviado ID: {idAsignado}");

                string confirmacion = NetworkStreamClass.LeerMensajeNetworkStream(stream);
                if (confirmacion == idAsignado.ToString())
                {
                    Console.WriteLine($"[Servidor] Handshake correcto con vehículo ID: {idAsignado}");
                }
                else
                {
                    Console.WriteLine("[Servidor] ERROR: ID confirmado no coincide.");
                }
            }

            cliente.Close();
        }
    }
}
