using System.Net;
using System.Net.Sockets;
using NetworkStreamNS;

namespace Servidor
{
    class Program
    {
        static TcpListener listener;
        static List<Cliente> clientesConectados = new List<Cliente>();
        static int contadorIds = 0;
        static object lockClientes = new object();

        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando servidor...");
            listener = new TcpListener(IPAddress.Any, 5000);
            listener.Start();

            Console.WriteLine("Servidor esperando conexiones...");

            while (true)
            {
                TcpClient clienteTcp = listener.AcceptTcpClient();
                Thread hilo = new Thread(() => GestionarCliente(clienteTcp));
                hilo.Start();
            }
        }

        static void GestionarCliente(TcpClient clienteTcp)
        {
            Console.WriteLine("Vehículo conectado. Gestionando nuevo vehículo...");

            NetworkStream ns = clienteTcp.GetStream();
            int idAsignado;

            lock (lockClientes)
            {
                idAsignado = ++contadorIds;
                Cliente nuevoCliente = new Cliente(idAsignado, ns);
                clientesConectados.Add(nuevoCliente);
                Console.WriteLine($"Vehículo ID {idAsignado} conectado. Total: {clientesConectados.Count}");
            }

            // Handshake
            string mensaje = NetworkStreamClass.LeerMensajeNetworkStream(ns);
            if (mensaje == "INICIO")
            {
                Console.WriteLine($"Handshake iniciado con el vehículo ID {idAsignado}");
                NetworkStreamClass.EscribirMensajeNetworkStream(ns, idAsignado.ToString());

                string confirmacion = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                if (confirmacion == idAsignado.ToString())
                {
                    Console.WriteLine($"Handshake completado con vehículo ID {idAsignado}");
                }
                else
                {
                    Console.WriteLine($"Error de handshake con vehículo ID {idAsignado}");
                }
            }

         
        }
    }
}
