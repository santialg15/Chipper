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
using Protocolo.Interfaces;
using Servidor;

namespace servidor
{
    class ServerHandler
    {
        static readonly ISettingsManager SettingsMgr = new SettingsManager();
        private readonly TcpClient _tcpClient;
        private readonly IFileStreamHandler _fileStreamHandler;
        private INetworkStreamHandler _networkStreamHandler;

        public ServerHandler()
        {
            _tcpClient = new TcpClient(new IPEndPoint(IPAddress.Parse(SettingsMgr.ReadSetting(ServerConfig.ServerIpConfigKey)), Int32.Parse(SettingsMgr.ReadSetting(ServerConfig.SeverPortTCPClifigKey))));
            _fileStreamHandler = new FileStreamHandler();
        }

        public void StartClient()
        {
            if (!_tcpClient.Connected)
            {
                _tcpClient.Connect(IPAddress.Parse(SettingsMgr.ReadSetting(ServerConfig.ServerIpConfigKey)), Int32.Parse(SettingsMgr.ReadSetting(ServerConfig.SeverPortTCPConnfigKey)));
                _networkStreamHandler = new NetworkStreamHandler(_tcpClient.GetStream());
            }
        }
        

        public void stop()
        {
            _tcpClient.Close();
            _tcpClient.Dispose();
        }

        public string ReceiveFile()
        {
            var header = _networkStreamHandler.Read(FileHeader.GetLength());
            
            var fileNameSize = BitConverter.ToInt32(header, 0);
            
            var fileSize = BitConverter.ToInt64(header, Specification.FixedFileNameLength);

            var fileName = Encoding.UTF8.GetString(_networkStreamHandler.Read(fileNameSize));

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