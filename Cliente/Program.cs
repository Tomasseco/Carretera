using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using NetworkStreamNS;
using VehiculoClass; // Necesario para usar la clase Vehiculo

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

                Console.WriteLine("Handshake completado. Creando vehículo...");

                // Crear el vehículo usando el ID recibido
                int idVehiculo = int.Parse(idRecibido);
                string direccion = (new Random().Next(0, 2) == 0) ? "Norte" : "Sur";

                Vehiculo miVehiculo = new Vehiculo(idVehiculo, direccion);

                Console.WriteLine($"Vehículo creado: ID={miVehiculo.Id}, Dirección={miVehiculo.Direccion}, Velocidad={miVehiculo.Velocidad}");

                // Enviar el vehículo al servidor
                NetworkStreamClass.EscribirDatosVehiculoNS(ns, miVehiculo);

                Console.WriteLine("Vehículo enviado al servidor.");

                // Aquí podrías esperar actualizaciones de la carretera si quieres
                // Por ahora cerramos
                ns.Close();
                cliente.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
