using System.Net.Sockets;
using System.Text;

namespace ProyectoCompartido.Protocolo
{
    public class NetworkDataHelper
    {
        private readonly Socket _socket;
        private readonly NetworkStream networkStream;


        public NetworkDataHelper(Socket socket)
        {
            _socket = socket;
        }

        public NetworkDataHelper(NetworkStream stream)
        {
            networkStream = stream;
        }

        public async Task SendMessage(string mensaje, int constant)
        {
            var header = new Header(HeaderConstants.Request, constant, mensaje.Length);
            var dataMessage = header.GetRequest();
            await networkStream.WriteAsync(dataMessage, 0, dataMessage.Length).ConfigureAwait(false);
            var bytesMessage = Encoding.UTF8.GetBytes(mensaje);
            await networkStream.WriteAsync(bytesMessage, 0, bytesMessage.Length).ConfigureAwait(false);
        }

        public async Task ReceiveData(byte[] buffer)
        {
            var Length2 = buffer.Length;
            var iRecv = 0;
            while (iRecv < Length2)
            {
                var localRecv = await networkStream.ReadAsync(buffer, iRecv, Length2 - iRecv).ConfigureAwait(false);
                if (localRecv == 0) // Si recieve retorna 0 -> la conexion se cerro desde el endpoint remoto
                {
                    throw new SocketException();
                }
                iRecv += localRecv;
                var dataLength = new byte[HeaderConstants.DataLength];
                var length = BitConverter.ToInt32(dataLength, 0);
                var data = new byte[length];
                var totalReceived = 0;
                while (totalReceived < length)
                {
                    var received = await networkStream.ReadAsync(data, totalReceived, length - totalReceived).ConfigureAwait(false);
                    if (received == 0)
                    {
                        throw new SocketException();
                    }
                    totalReceived += received;
                }
            }
        }

    }
}
