﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetworkStreamNS;
using VehiculoClass;
using CarreteraClass;

namespace Servidor
{
    class Program
    {
        static TcpListener listener;
        static List<Cliente> clientesConectados = new List<Cliente>();
        static int contadorIds = 0;
        static object lockClientes = new object();
        static Carretera carretera = new Carretera();
        static object lockCarretera = new object();

        // Sincronización del puente
        static int? vehiculoEnPuenteId = null;
        static object lockPuente = new object();

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

            string mensaje = NetworkStreamClass.LeerMensajeNetworkStream(ns);
            if (mensaje == "INICIO")
            {
                Console.WriteLine($"Handshake iniciado con el vehículo ID {idAsignado}");
                NetworkStreamClass.EscribirMensajeNetworkStream(ns, idAsignado.ToString());

                string confirmacion = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                if (confirmacion == idAsignado.ToString())
                {
                    Console.WriteLine($"Handshake completado con vehículo ID {idAsignado}");

                    Vehiculo nuevoVehiculo = NetworkStreamClass.LeerDatosVehiculoNS(ns);
                    Console.WriteLine($"Vehículo recibido: ID={nuevoVehiculo.Id}, Dirección={nuevoVehiculo.Direccion}, Velocidad={nuevoVehiculo.Velocidad}");
                    Console.Clear();

                    lock (lockCarretera)
                    {
                        carretera.AñadirVehiculo(nuevoVehiculo);
                        carretera.MostrarVehiculos();
                        EnviarCarreteraATodos();
                    }

                    bool terminado = false;
                    while (!terminado)
                    {
                        Vehiculo vehiculoActualizado = NetworkStreamClass.LeerDatosVehiculoNS(ns);

                        lock (lockPuente)
                        {
                            if (vehiculoActualizado.Pos >= 30 && vehiculoActualizado.Pos <= 50)
                            {
                                while (vehiculoEnPuenteId != null && vehiculoEnPuenteId != vehiculoActualizado.Id)
                                {
                                    vehiculoActualizado.Parado = true;

                                    lock (lockCarretera)
                                    {
                                        carretera.ActualizarVehiculo(vehiculoActualizado);
                                        carretera.MostrarVehiculos();
                                        EnviarCarreteraATodos();
                                    }

                                    Monitor.Wait(lockPuente);
                                }

                                vehiculoEnPuenteId = vehiculoActualizado.Id;
                                vehiculoActualizado.Parado = false;
                            }
                            else
                            {
                                if (vehiculoEnPuenteId == vehiculoActualizado.Id && vehiculoActualizado.Pos > 50)
                                {
                                    vehiculoEnPuenteId = null;
                                    Monitor.PulseAll(lockPuente);
                                }

                                vehiculoActualizado.Parado = false;
                            }
                        }

                        lock (lockCarretera)
                        {
                            carretera.ActualizarVehiculo(vehiculoActualizado);
                            carretera.MostrarVehiculos();
                            EnviarCarreteraATodos();
                        }

                        if (vehiculoActualizado.Acabado)
                        {
                            terminado = true;
                            Console.WriteLine($"Vehículo ID {vehiculoActualizado.Id} ha terminado su recorrido.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Error de handshake con vehículo ID {idAsignado}");
                }
            }
        }

        static void EnviarCarreteraATodos()
        {
            lock (lockClientes)
            {
                List<Cliente> clientesDesconectados = new List<Cliente>();

                foreach (var cliente in clientesConectados)
                {
                    try
                    {
                        if (cliente.NS != null && cliente.NS.CanWrite)
                        {
                            NetworkStreamClass.EscribirDatosCarreteraNS(cliente.NS, carretera);
                        }
                        else
                        {
                            clientesDesconectados.Add(cliente);
                        }
                    }
                    catch (Exception)
                    {
                        clientesDesconectados.Add(cliente);
                    }
                }

                foreach (var cliente in clientesDesconectados)
                {
                    clientesConectados.Remove(cliente);
                    Console.WriteLine($"Cliente ID {cliente.Id} desconectado y eliminado de la lista.");
                }
            }
        }
    }
}
