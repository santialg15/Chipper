using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente
{
    public static class ClientConf
    {
        public static string clientCntCarPubConfigKey = "CntCarPub"; //caracteres por publicación
        public static string clientCntImgPubConfigKey = "CntImgPub"; // Imagenes por publicación

        public static string ServerIpConfigKey = "ServerIpAddress";
        public static string SeverPortConfigKey = "ServerPort";
        public static string SeverPortTCPLiCofigKey = "ServerTCPLiPort";

        public static string ClientIpConfigKey = "ClientIpAddress";
    }
}
