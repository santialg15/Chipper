using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Protocolo.FileHandler.Interfaces;
using Protocolo.FileHandler;
using Protocolo.Interfaces;
using Protocolo.NetworkUtils;
using Protocolo.NetworkUtils.Interfaces;

namespace Protocolo
{
    public class NetworkDataHelper
    {
        private readonly Socket _socket;


        public NetworkDataHelper(Socket socket)
        {
            _socket = socket;
        }

        public void SendMessage(string mensaje, int constant)
        {
            var header = new Header(HeaderConstants.Request, constant, mensaje.Length);
            var dataMessage = header.GetRequest();
            var sentBytes = 0;
            while (sentBytes < dataMessage.Length)
            {
                sentBytes += _socket.Send(dataMessage, sentBytes, dataMessage.Length - sentBytes, SocketFlags.None);
            }

            sentBytes = 0;
            var bytesMessage = Encoding.UTF8.GetBytes(mensaje);
            while (sentBytes < bytesMessage.Length)
            {
                sentBytes += _socket.Send(bytesMessage, sentBytes, bytesMessage.Length - sentBytes,
                    SocketFlags.None);
            }
        }

        public void ReceiveData(int Length, byte[] buffer, bool _exit)
        {
            var iRecv = 0;
            while (iRecv < Length)
            {
                try
                {
                    var localRecv = _socket.Receive(buffer, iRecv, Length - iRecv, SocketFlags.None);
                    if (localRecv == 0) 
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
            }
        }

    }
}
