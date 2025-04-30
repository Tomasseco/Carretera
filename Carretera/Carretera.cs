using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Threading;

namespace CarreteraClass
{
    [Serializable]
    public class Carretera
    {
        [XmlIgnore]
        public List<VehiculoClass.Vehiculo> VehiculosEnCarretera { get; set; }

        public int NumVehiculosEnCarrera { get; set; }
        private static object lockPuente = new object();

        public Carretera()
        {
            VehiculosEnCarretera = new List<VehiculoClass.Vehiculo>();
            NumVehiculosEnCarrera = 0;
        }

        public byte[] CarreteraaBytes()
        {
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
            return deserializada;
        }

        // Método para obtener la lista de vehículos
        public List<VehiculoClass.Vehiculo> ObtenerListaVehiculos()
        {
            return new List<VehiculoClass.Vehiculo>(VehiculosEnCarretera);
        }

        public void MostrarVehiculos()
        {
            Console.Clear();
            Console.WriteLine("=== Estado actual de los vehículos en la carretera ===\n");

            foreach (var v in VehiculosEnCarretera)
            {
                string estado = v.Acabado ? "Finalizado" : v.Parado ? "Esperando" : "Cruzando";
                string progreso = new string('█', v.Pos / 10) + new string('▒', 10 - (v.Pos / 10));
                Console.WriteLine($"[{v.Direccion}] Vehículo #{v.Id,-2}: {progreso} (km {v.Pos,3} - {estado})");
            }
        }

        public void AñadirVehiculo(VehiculoClass.Vehiculo v)
        {
            VehiculosEnCarretera.Add(v);
            NumVehiculosEnCarrera = VehiculosEnCarretera.Count;
        }

        public void ActualizarVehiculo(VehiculoClass.Vehiculo vActualizado)
        {
            lock (lockPuente)
            {
                // Si el vehículo está en el puente (km 30-50), solo este podrá avanzar
                if (vActualizado.EstaEnPuente())
                {
                    // Pausar todos los vehículos que no están en el puente
                    foreach (var v in VehiculosEnCarretera)
                    {
                        if (v.Id != vActualizado.Id)
                        {
                            v.Parado = true;
                        }
                    }

                    // Dejar avanzar el vehículo actual en el puente
                    vActualizado.Parado = false;
                }
                else
                {
                    // Si no está en el puente, puede avanzar
                    vActualizado.Parado = false;
                }

                // Actualizar la lista de vehículos
                var copia = new List<VehiculoClass.Vehiculo>(VehiculosEnCarretera);
                VehiculosEnCarretera.Clear();
                foreach (var v in copia)
                {
                    if (v.Id == vActualizado.Id)
                    {
                        VehiculosEnCarretera.Add(vActualizado);
                    }
                    else
                    {
                        VehiculosEnCarretera.Add(v);
                    }
                }

                NumVehiculosEnCarrera = VehiculosEnCarretera.Count;
            }
        }
    }
}
