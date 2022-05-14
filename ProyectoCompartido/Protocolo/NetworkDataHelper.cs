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

        public void SendMessage(string mensaje, int constant)
        {
            var header = new Header(HeaderConstants.Request, constant, mensaje.Length);
            var dataMessage = header.GetRequest();
            var sentBytes = 0;
            //while (sentBytes < dataMessage.Length)
            //{
                networkStream.Write(dataMessage, 0, sentBytes);
                //sentBytes += _socket.Send(dataMessage, sentBytes, dataMessage.Length - sentBytes, SocketFlags.None);
            //}

            sentBytes = 0;
            var bytesMessage = Encoding.UTF8.GetBytes(mensaje);
            //while (sentBytes < bytesMessage.Length)
            //{
                networkStream.Write(bytesMessage, 0, sentBytes);
                //sentBytes += _socket.Send(bytesMessage, sentBytes, bytesMessage.Length - sentBytes, SocketFlags.None);
            //}
        }

        public void ReceiveData(int Length, byte[] buffer, bool _exit)
        {
            var iRecv = 0;
            while (iRecv < Length)
            {
                try
                {
                    var localRecv = networkStream.Read(buffer, iRecv, Length - iRecv);
                    //var localRecv = _socket.Receive(buffer, iRecv, Length - iRecv, SocketFlags.None);
                    if (localRecv == 0) // Si recieve retorna 0 -> la conexion se cerro desde el endpoint remoto
                    {
                        if (!_exit)
                        {
                            _socket.Shutdown(SocketShutdown.Both);
                            _socket.Close();
                        }
                        else
                        {
                            throw new Exception("Server is closing");
                        }
                    }

                    iRecv += localRecv;
                }

                catch (SocketException se)
                {
                    Console.WriteLine(se.Message);
                    return;
                }
                var length = BitConverter.ToInt32(buffer, 0);
                var data = new byte[length];
                var totalReceived = 0;
                while (totalReceived < length)
                {
                    var received = networkStream.Read(data, totalReceived, length - totalReceived);
                    if (received == 0)
                    {
                        throw new SocketException();
                    }
                    totalReceived += received;
                }
                var word = Encoding.UTF8.GetString(data);
                if (word.Equals("exit"))
                {
                    _exit = false;
                    Console.WriteLine("Client is leaving");
                }
                else
                {
                    Console.WriteLine("Client says: " + word);
                }
            }
        }

    }
}
