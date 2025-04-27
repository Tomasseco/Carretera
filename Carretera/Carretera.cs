using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;

namespace CarreteraClass
{
    [Serializable]
    public class Carretera
    {
        [XmlIgnore]
        public PriorityQueue<VehiculoClass.Vehiculo, int> VehiculosEnCarretera { get; set; }

        public int NumVehiculosEnCarrera { get; set; }

        public Carretera()
        {
            VehiculosEnCarretera = new PriorityQueue<VehiculoClass.Vehiculo, int>();
            NumVehiculosEnCarrera = 0;
        }

        // Serializar Carretera a array de bytes
        public byte[] CarreteraaBytes()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Carretera));
            MemoryStream ms = new MemoryStream();
            serializer.Serialize(ms, this);
            return ms.ToArray();
        }

        // Deserializar array de bytes a objeto Carretera
        public static Carretera BytesACarretera(byte[] bytesCarretera)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Carretera));
            MemoryStream ms = new MemoryStream(bytesCarretera);
            return (Carretera)serializer.Deserialize(ms);
        }

        // Mostrar todos los vehículos de la carretera
        public void MostrarVehiculos()
        {
            Console.Clear();
            // Como no podemos enumerar directamente una PriorityQueue, clonamos para mostrar
            var copiaVehiculos = new List<VehiculoClass.Vehiculo>();

            while (VehiculosEnCarretera.Count > 0)
            {
                VehiculosEnCarretera.TryDequeue(out var vehiculo, out _);
                copiaVehiculos.Add(vehiculo);
            }

            // Mostrar y reconstruir la cola
            foreach (var v in copiaVehiculos)
            {
                string estado = v.Parado ? "Esperando" : v.Acabado ? "Finalizado" : "Cruzando";
                string progreso = new string('█', v.Pos / 10) + new string('▒', 10 - (v.Pos / 10));

                Console.WriteLine($"[{v.Direccion}] Vehículo #{v.Id}: {progreso} (km {v.Pos} - {estado})");

                // Volvemos a encolar
                VehiculosEnCarretera.Enqueue(v, PrioridadVehiculo(v));
            }
        }

        // Añadir un vehículo a la carretera
        public void AñadirVehiculo(VehiculoClass.Vehiculo v)
        {
            VehiculosEnCarretera.Enqueue(v, PrioridadVehiculo(v));
            NumVehiculosEnCarrera = VehiculosEnCarretera.Count;
        }

        // Actualizar un vehículo (lo quitamos todo y lo reencolamos)
        public void ActualizarVehiculo(VehiculoClass.Vehiculo vActualizado)
        {
            var copiaVehiculos = new List<VehiculoClass.Vehiculo>();

            while (VehiculosEnCarretera.Count > 0)
            {
                VehiculosEnCarretera.TryDequeue(out var vehiculo, out _);
                if (vehiculo.Id != vActualizado.Id)
                {
                    copiaVehiculos.Add(vehiculo);
                }
            }

            // Volvemos a añadir todos los demás
            foreach (var v in copiaVehiculos)
            {
                VehiculosEnCarretera.Enqueue(v, PrioridadVehiculo(v));
            }

            // Añadimos actualizado
            VehiculosEnCarretera.Enqueue(vActualizado, PrioridadVehiculo(vActualizado));

            NumVehiculosEnCarrera = VehiculosEnCarretera.Count;
        }

        // Calcular la prioridad de un vehículo (inversa de la posición para que avance el que más lejos va)
        private int PrioridadVehiculo(VehiculoClass.Vehiculo v)
        {
            return -v.Pos; // Negativo para que el de mayor Pos aparezca primero
        }
    }
}
