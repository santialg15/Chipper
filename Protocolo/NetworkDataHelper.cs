using System;
using System.Net.Sockets;
using System.Text;


namespace Protocolo
{
    public class NetworkDataHelper
    {
        private readonly TcpClient _tcpClient;
        public NetworkDataHelper(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
        }

        public void SendMessage(TcpClient tcpClient, string mensaje, int constant)
        {

            var networkStream = _tcpClient.GetStream();
            var header = new Header(HeaderConstants.Request, constant, mensaje.Length);
            var dataMessage = header.GetRequest();
            //byte[] headerBytes = BitConverter.GetBytes(dataMessage.Length);

            byte[] data = Encoding.UTF8.GetBytes(mensaje);

            networkStream.Write(dataMessage, 0, dataMessage.Length);
            networkStream.Write(data, 0, data.Length);
        }

        public (string, Header) ReceiveData()
        {
            Header header = new Header();
            var word = "";
            try
            {
                NetworkStream networkStream = _tcpClient.GetStream();
                //Obtengo el header
                var dataLength = new byte[HeaderConstants.DataLength + HeaderConstants.CommandLength +
                                          HeaderConstants.Request.Length];
                var totalReceived = 0;
                while (totalReceived < HeaderConstants.DataLength)
                {
                    var received = networkStream.Read(dataLength, totalReceived, dataLength.Length);
                    if (received ==
                        0) // if receive 0 bytes this means that connection was interrupted between the two points
                    {
                        throw new SocketException();
                    }

                    totalReceived += received;
                }

                header.DecodeData(dataLength);


                //Obtengo los datos enviados
                var length = header.IDataLength; // largo del mensaje que viene en header
                var data = new byte[length];
                totalReceived = 0;
                while (totalReceived < length)
                {
                    var received = networkStream.Read(data, totalReceived, length);
                    if (received == 0)
                    {
                        throw new SocketException();
                    }

                    totalReceived += received;
                }

                word = Encoding.UTF8.GetString(data);
            }
            catch (SocketException)
            {
                Console.WriteLine("The client connection was interrupted");
            }
            return (word, header);
        }

    }
}
