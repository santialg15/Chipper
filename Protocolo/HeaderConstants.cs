namespace Protocolo
{
    public static class HeaderConstants
    {
        public static string Request = "REQ";
        public static string Response = "RES";
        public static int CommandLength = 2;
        public static int DataLength = 4;
    }
}


// Envio del client al server -> REQXXYYYY -> REQ -> sentido de la operacion
//                                                -> XX -> Numero de Comando (00-99)
//                                                -> YYYY -> 0000 - 9999