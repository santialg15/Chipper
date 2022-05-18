using System;
using System.Net.Sockets;
using System.Text;
using Protocolo.FileHandler.Interfaces;
using Protocolo.FileTransfer;
using Protocolo.FileTransfer.FileHandler;
using Protocolo.NetworkUtils;
using Protocolo.NetworkUtils.Interfaces;

namespace Protocolo
{
    public class NetworkDataHelper
    {
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _networkStream;
        //SendFile
        private readonly IFileStreamHandler _fileStreamHandler;
        private IFileHandler _fileHandler;
        //Sendrecieve
        private INetworkStreamHandler _networkStreamHandler;

        public NetworkDataHelper(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            _networkStream = tcpClient.GetStream();
            _networkStreamHandler = new NetworkStreamHandler(_networkStream); //ver si usar este siempre por cumplir con protocolo
            _fileStreamHandler = new FileStreamHandler();
        }

        public void SendMessage(string mensaje, int constant)
        {

            var networkStream = _tcpClient.GetStream();
            var header = new Header(HeaderConstants.Request, constant, mensaje.Length);
            var dataMessage = header.GetRequest();
            //byte[] headerBytes = BitConverter.GetBytes(dataMessage.Length);

            byte[] data = Encoding.UTF8.GetBytes(mensaje);

            _networkStream.Write(dataMessage, 0, dataMessage.Length);
            _networkStream.Write(data, 0, data.Length);
        }

        public (string, Header) ReceiveData()
        {
            Header header = new Header();
            var word = "";
            try
            {
                
                //Obtengo el header
                var dataLength = new byte[HeaderConstants.DataLength + HeaderConstants.CommandLength +
                                          HeaderConstants.Request.Length];
                var totalReceived = 0;
                while (totalReceived < HeaderConstants.DataLength)
                {
                    var received = _networkStream.Read(dataLength, totalReceived, dataLength.Length);
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
                    var received = _networkStream.Read(data, totalReceived, length);
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

        
        public void SendFile(string path)
        {
            _fileHandler = new FileTransfer.FileHandler.FileHandler();

            // Obtiene el tamaño del archivo
            long fileSize = _fileHandler.GetFileSize(path);
            // Obtenemos el nombre del archivo
            string fileName = _fileHandler.GetFileName(path);
            var header = new FileHeader().Create(fileName, fileSize);
            _networkStreamHandler.Write(header); // envia largo del nombre y tamaño del file

            _networkStreamHandler.Write(Encoding.UTF8.GetBytes(fileName)); // Envia nombre del archivo

            long parts = SpecificationHelper.GetParts(fileSize);
            Console.WriteLine("Will Send {0} parts", parts);
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
                    // leo el ultimo segmento
                    var lastPartSize = (int)(fileSize - offset);
                    data = _fileStreamHandler.Read(path, offset, lastPartSize); //Puntos 1 y 2 
                    offset += lastPartSize;
                }
                else
                {
                    // leo un segmento
                    data = _fileStreamHandler.Read(path, offset, Specification.MaxPacketSize); //Puntos 1 y 2
                    offset += Specification.MaxPacketSize;
                }

                _networkStreamHandler.Write(data); // Punto 3
                currentPart++;
            }
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
