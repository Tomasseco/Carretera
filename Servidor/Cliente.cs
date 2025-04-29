using System.Net.Sockets;

namespace Servidor
{
    public class Cliente
    {
        public int Id { get; }
        public NetworkStream NS { get; }

        public Cliente(int id, NetworkStream ns)
        {
            Id = id;
            NS = ns;
        }
    }
}