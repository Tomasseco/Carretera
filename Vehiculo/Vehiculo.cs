using System.Xml.Serialization;
using System.IO;

namespace VehiculoClass
{
    [Serializable]
    public class Vehiculo
    {
        public int Id { get; set; }
        public int Pos { get; set; }
        public int Velocidad { get; set; }
        public bool Acabado { get; set; }
        public string Direccion { get; set; } // "Norte" o "Sur"
        public bool Parado { get; set; }

        public Vehiculo()
        {
            Random randVelocidad = new Random();
            Velocidad = randVelocidad.Next(100, 501);
            Pos = 0;
            Acabado = false;
            Parado = false;
            Direccion = "Norte"; 
        }

        public Vehiculo(int id, string direccion)
        {
            Id = id;
            Direccion = direccion;
            Random randVelocidad = new Random();
            Velocidad = randVelocidad.Next(100, 501);
            Pos = 0;
            Acabado = false;
            Parado = false;
        }

        public void Avanzar()
        {
            if (!Parado && !Acabado)
            {
                Pos++;

                if (Pos >= 100)
                {
                    Acabado = true;
                }
            }
        }

        // Permite serializar Vehiculo a array de bytes mediante formato XML
        public byte[] VehiculoaBytes()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Vehiculo));
            MemoryStream ms = new MemoryStream();
            serializer.Serialize(ms, this);
            return ms.ToArray();
        }

        // Permite deserializar una cadena de bytes a un objeto de tipo Vehiculo
        public static Vehiculo BytesAVehiculo(byte[] bytesVehiculo)
        {
            Vehiculo tmpVehiculo;
            XmlSerializer serializer = new XmlSerializer(typeof(Vehiculo));
            MemoryStream ms = new MemoryStream(bytesVehiculo);
            tmpVehiculo = (Vehiculo)serializer.Deserialize(ms);
            return tmpVehiculo;
        }
    }
}
