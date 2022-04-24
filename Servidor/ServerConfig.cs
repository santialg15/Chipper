namespace Servidor
{
    public static class ServerConfig
    {
        public static string ServerIpConfigKey = "ServerIpAddress";
        public static string SeverPortConfigKey = "ServerPort";
        public static string SeverPortTCPConnfigKey = "ServerTCPConnPort";
        public static string SeverPortTCPClifigKey = "ServerTCPCliPort";


        // SRF6
        public static string SeverTopSeguidoresConfigKey = "TopSeguidores";

        // SRF7
        public static string SeverTmpMostrarPubConfigKey = "TmpMostrarPub"; // umbral en minutos top x que más usaron el sistema
        public static string SeverTopMasUsosConfigKey = "TopMasUsos"; // top x que más usaron el sistema
    }
}