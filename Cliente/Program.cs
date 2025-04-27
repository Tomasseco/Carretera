using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using NetworkStreamNS;
using VehiculoClass;

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
                NetworkStreamClass.EscribirMensajeNetworkStream(ns, "INICIO");

                string idRecibido = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                Console.WriteLine($"ID recibido del servidor: {idRecibido}");

                NetworkStreamClass.EscribirMensajeNetworkStream(ns, idRecibido);

                Console.WriteLine("Handshake completado. Creando vehículo...");

                Vehiculo miVehiculo = new Vehiculo(int.Parse(idRecibido), "Norte");
                NetworkStreamClass.EscribirDatosVehiculoNS(ns, miVehiculo);

                while (!miVehiculo.Acabado)
                {
                    miVehiculo.Pos++;

                    if (miVehiculo.Pos >= 100)
                    {
                        miVehiculo.Acabado = true;
                    }

                    NetworkStreamClass.EscribirDatosVehiculoNS(ns, miVehiculo);

                    Thread.Sleep(miVehiculo.Velocidad);
                }

                Console.WriteLine("Vehículo ha terminado su recorrido.");
                cliente.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
