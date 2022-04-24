using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Protocolo.FileHandler;
using Protocolo.FileHandler.Interfaces;
using Protocolo.NetworkUtils;
using Protocolo.NetworkUtils.Interfaces;
using Protocolo;
using Protocolo.FileTransfer;

namespace servidor
{
    class ServerHandler
    {
        
        private readonly TcpClient _tcpClient;
        private readonly IFileStreamHandler _fileStreamHandler;
        private INetworkStreamHandler _networkStreamHandler;

        public ServerHandler()
        {
            _tcpClient = new TcpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6001));
            _fileStreamHandler = new FileStreamHandler();
        }

        public void StartClient()
        {
            _tcpClient.Connect(IPAddress.Parse("127.0.0.1"), 6000);
            _networkStreamHandler = new NetworkStreamHandler(_tcpClient.GetStream());
        }

        public string ReceiveFile()
        {
            //1 - Recibo el header
            var header = _networkStreamHandler.Read(FileHeader.GetLength());
            // 2 - Me quedo con el largo del nombre del archivo
            var fileNameSize = BitConverter.ToInt32(header, 0);
            // 3 - Me quedo con el tamaño del file
            var fileSize = BitConverter.ToInt64(header, Specification.FixedFileNameLength);

            //4 - Recibo el nombre del archivo, usando el tamaño que recibi en el punto 2
            var fileName = Encoding.UTF8.GetString(_networkStreamHandler.Read(fileNameSize));

            // 5 - Calculo cuantas partes voy a recibir
            long parts = SpecificationHelper.GetParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            // Mientras tengo partes para recibir
            // 1 - Me fijo si es la ultima parte
            // 1.1 - Si es, recibo la ultima parte
            // 2.2 - Si no, recibo una parte cualquiera
            // 3 - Escribo esa parte del archivo a disco
            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == parts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    data = _networkStreamHandler.Read(lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = _networkStreamHandler.Read(Specification.MaxPacketSize);
                    offset += Specification.MaxPacketSize;
                }
                _fileStreamHandler.Write(fileName, data);
                currentPart++;
            }

            return fileName;
        }
    }
}