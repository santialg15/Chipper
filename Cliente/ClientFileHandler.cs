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

namespace cliente
{
    class ClientFileHandler
    {
        private readonly TcpListener _tcpListener;
        private readonly IFileHandler _fileHandler;
        private readonly IFileStreamHandler _fileStreamHandler;
        private TcpClient _tcpClient;
        private INetworkStreamHandler _networkStreamHandler;

        public ClientFileHandler()
        {
            _tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 6000);
            _fileHandler = new FileHandler();
            _fileStreamHandler = new FileStreamHandler();
        }

        public void StartServer()
        {
            _tcpListener.Start(1);
            _tcpClient = _tcpListener.AcceptTcpClient();
            _tcpListener.Stop();
            _networkStreamHandler = new NetworkStreamHandler(_tcpClient.GetStream());
        }

        public void stop()
        {
            _tcpClient.Close();
            _tcpClient.Dispose();
            _tcpListener.Stop();
        }

        public void SendFile(string path)
        {
            //1) Obtiene el tamaño del archivo
            long fileSize = _fileHandler.GetFileSize(path);
            //2) Obtenemos el nombre del archivo
            string fileName = _fileHandler.GetFileName(path);
            var header = new FileHeader().Create(fileName, fileSize);
            _networkStreamHandler.Write(header); // Aca enviamos largo del nombre y tamaño del file
            
            _networkStreamHandler.Write(Encoding.UTF8.GetBytes(fileName)); // Enviamos el nombre del archivo

            long parts = SpecificationHelper.GetParts(fileSize);
            Console.WriteLine("Will Send {0} parts",parts);
            long offset = 0;
            long currentPart = 1;

            // Mientras tengo un segmento a enviar
            // 1 - Leo de disco el segmento
            // 2 - Guardo ese segmento en un buffer
            // 3 - Envio ese semgento a traves de la red
            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == parts)
                {
                    // leo el ultimo segmento del archivo
                    var lastPartSize = (int)(fileSize - offset);
                    data = _fileStreamHandler.Read(path, offset, lastPartSize); //Puntos 1 y 2 
                    offset += lastPartSize;
                }
                else
                {
                    // leo un segmento del archivo
                    data = _fileStreamHandler.Read(path, offset, Specification.MaxPacketSize); //Puntos 1 y 2
                    offset += Specification.MaxPacketSize;
                }

                _networkStreamHandler.Write(data); // Punto 3
                currentPart++;
            }
        }
    }
}