using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using VehiculoClass;
using CarreteraClass;

namespace NetworkStreamNS
{
    public class NetworkStreamClass
    {
        // Para NetworkStream los datos de tipo Carretera
        public static void EscribirDatosCarreteraNS(NetworkStream NS, Carretera C)
        {
            byte[] datos = C.CarreteraaBytes();

            // Primero enviamos la longitud del array
            byte[] longitud = BitConverter.GetBytes(datos.Length);
            NS.Write(longitud, 0, longitud.Length);

            // Luego enviamos los datos
            NS.Write(datos, 0, datos.Length);
        }

        // Método para leer de un NetworkStream los datos que forman un objeto Carretera
        public static Carretera LeerDatosCarreteraNS(NetworkStream NS)
        {
            byte[] bufferLongitud = new byte[sizeof(int)];
            NS.Read(bufferLongitud, 0, bufferLongitud.Length);
            int longitud = BitConverter.ToInt32(bufferLongitud, 0);

            byte[] datos = new byte[longitud];
            int totalLeidos = 0;
            while (totalLeidos < longitud)
            {
                int bytesLeidos = NS.Read(datos, totalLeidos, longitud - totalLeidos);
                if (bytesLeidos == 0)
                    throw new IOException("Conexión cerrada inesperadamente");
                totalLeidos += bytesLeidos;
            }

            return Carretera.BytesACarretera(datos);
        }

        // Metodo para enviar datos de tipo Vehiculo en un NetworkStream
        public static void EscribirDatosVehiculoNS(NetworkStream NS, Vehiculo V)
        {
            byte[] datos = V.VehiculoaBytes();

            // Primero enviamos la longitud del array
            byte[] longitud = BitConverter.GetBytes(datos.Length);
            NS.Write(longitud, 0, longitud.Length);

            // Luego enviamos los datos
            NS.Write(datos, 0, datos.Length);
        }

        // Método para leer de un NetworkStream los datos que forman un objeto Vehiculo
        public static Vehiculo LeerDatosVehiculoNS(NetworkStream NS)
        {
            byte[] bufferLongitud = new byte[sizeof(int)];
            NS.Read(bufferLongitud, 0, bufferLongitud.Length);
            int longitud = BitConverter.ToInt32(bufferLongitud, 0);

            byte[] datos = new byte[longitud];
            int totalLeidos = 0;
            while (totalLeidos < longitud)
            {
                int bytesLeidos = NS.Read(datos, totalLeidos, longitud - totalLeidos);
                if (bytesLeidos == 0)
                    throw new IOException("Conexión cerrada inesperadamente");
                totalLeidos += bytesLeidos;
            }

            return Vehiculo.BytesAVehiculo(datos);
        }

        // Método que permite leer un mensaje de tipo texto de un NetworkStream
        public static string LeerMensajeNetworkStream(NetworkStream NS)
        {
            byte[] bufferLectura = new byte[1024];

            // leo el mensaje
            int bytesLeidos = 0;
            var tmpStream = new MemoryStream();
            byte[] bytesTotales;
            do
            {
                int bytesLectura = NS.Read(bufferLectura, 0, bufferLectura.Length);
                tmpStream.Write(bufferLectura, 0, bytesLectura);
                bytesLeidos = bytesLeidos + bytesLectura;
            } while (NS.DataAvailable);

            bytesTotales = tmpStream.ToArray();

            return Encoding.Unicode.GetString(bytesTotales, 0, bytesLeidos);
        }

        // metodo que permite escribir un mensaje de tipo texto al NetworkStream
        public static void EscribirMensajeNetworkStream(NetworkStream NS, string Str)
        {
            byte[] MensajeBytes = Encoding.Unicode.GetBytes(Str);
            NS.Write(MensajeBytes, 0, MensajeBytes.Length);
        }
    }
}
