using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CarreteraClass
{
    [Serializable]
    public class Carretera
    {
        [XmlIgnore]
        public PriorityQueue<VehiculoClass.Vehiculo, int> VehiculosEnCarretera { get; set; }

        // Campo auxiliar para serialización
        public List<VehiculoClass.Vehiculo> ListaVehiculos { get; set; }

        public int NumVehiculosEnCarrera { get; set; }

        public Carretera()
        {
            VehiculosEnCarretera = new PriorityQueue<VehiculoClass.Vehiculo, int>();
            ListaVehiculos = new List<VehiculoClass.Vehiculo>();
            NumVehiculosEnCarrera = 0;
        }

        public byte[] CarreteraaBytes()
        {
            ListaVehiculos = ObtenerListaVehiculos();
            XmlSerializer serializer = new XmlSerializer(typeof(Carretera));
            using MemoryStream ms = new MemoryStream();
            serializer.Serialize(ms, this);
            return ms.ToArray();
        }

        public static Carretera BytesACarretera(byte[] bytesCarretera)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Carretera));
            using MemoryStream ms = new MemoryStream(bytesCarretera);
            Carretera deserializada = (Carretera)serializer.Deserialize(ms);

            // Reconstruir la cola
            deserializada.VehiculosEnCarretera = new PriorityQueue<VehiculoClass.Vehiculo, int>();
            foreach (var v in deserializada.ListaVehiculos)
            {
                deserializada.VehiculosEnCarretera.Enqueue(v, deserializada.PrioridadVehiculo(v));
            }

            return deserializada;
        }

        public void MostrarVehiculos()
        {
            var copiaVehiculos = ObtenerListaVehiculos();
            Console.SetCursorPosition(0, 0);
            foreach (var v in copiaVehiculos)
            {
                string estado = v.Parado ? "Esperando" : v.Acabado ? "Finalizado" : "Cruzando";
                string progreso = new string('█', v.Pos / 10) + new string('▒', 10 - (v.Pos / 10));
                Console.WriteLine($"[{v.Direccion}] Vehículo #{v.Id}: {progreso} (km {v.Pos} - {estado})");
            }
        }

        public void AñadirVehiculo(VehiculoClass.Vehiculo v)
        {
            VehiculosEnCarretera.Enqueue(v, PrioridadVehiculo(v));
            NumVehiculosEnCarrera = VehiculosEnCarretera.Count;
        }

        public void ActualizarVehiculo(VehiculoClass.Vehiculo vActualizado)
        {
            var copia = new List<VehiculoClass.Vehiculo>();
            while (VehiculosEnCarretera.Count > 0)
            {
                VehiculosEnCarretera.TryDequeue(out var v, out _);
                if (v.Id != vActualizado.Id)
                    copia.Add(v);
            }

            foreach (var v in copia)
                VehiculosEnCarretera.Enqueue(v, PrioridadVehiculo(v));

            VehiculosEnCarretera.Enqueue(vActualizado, PrioridadVehiculo(vActualizado));
            NumVehiculosEnCarrera = VehiculosEnCarretera.Count;
        }

        private int PrioridadVehiculo(VehiculoClass.Vehiculo v)
        {
            return -v.Pos;
        }

        // Método para obtener una lista enumerando los vehículos de la cola
        public List<VehiculoClass.Vehiculo> ObtenerListaVehiculos()
        {
            var lista = new List<VehiculoClass.Vehiculo>();
            var copia = new List<VehiculoClass.Vehiculo>();

            while (VehiculosEnCarretera.Count > 0)
            {
                VehiculosEnCarretera.TryDequeue(out var v, out _);
                lista.Add(v);
                copia.Add(v);
            }

            // Reconstruir la cola original
            foreach (var v in copia)
                VehiculosEnCarretera.Enqueue(v, PrioridadVehiculo(v));

            return lista;
        }
    }
}
