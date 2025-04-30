using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using NetworkStreamNS;
using VehiculoClass;
using CarreteraClass;

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

                // Asignar dirección aleatoria y velocidad aleatoria
                Random rnd = new Random();
                string[] direcciones = { "Norte", "Sur" };
                string direccion = direcciones[rnd.Next(direcciones.Length)];
                int velocidad = rnd.Next(500, 1001); 

                Vehiculo miVehiculo = new Vehiculo(int.Parse(idRecibido), direccion);
                NetworkStreamClass.EscribirDatosVehiculoNS(ns, miVehiculo);

                Thread hiloEscucha = new Thread(() =>
                {
                    try
                    {
                        while (true)
                        {
                            Carretera carreteraRecibida = NetworkStreamClass.LeerDatosCarreteraNS(ns);

                            Console.Clear();
                           foreach (var v in carreteraRecibida.ObtenerListaVehiculos())
                            {
                                string estado;

                                if (v.Acabado)
                                    estado = "Finalizado";
                                else if (v.Parado)
                                    estado = "Esperando";
                                else
                                    estado = "Cruzando";

                                string progreso = new string('█', v.Pos / 10) + new string('▒', 10 - (v.Pos / 10));
                                Console.WriteLine($"[{v.Direccion}] Vehículo #{v.Id}: {progreso} (km {v.Pos} - {estado})");
                            }
                        }
                    }
                    catch
                    {
                    
                    }
                });

                hiloEscucha.IsBackground = true;
                hiloEscucha.Start();

                while (!miVehiculo.Acabado)
                {
                    if (!miVehiculo.Parado)
                    {
                        miVehiculo.Pos++;

                        if (miVehiculo.Pos >= 100)
                            miVehiculo.Acabado = true;

                        NetworkStreamClass.EscribirDatosVehiculoNS(ns, miVehiculo);
                    }

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
