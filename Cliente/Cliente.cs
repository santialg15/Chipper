using System.Net;
using System.Net.Sockets;
using System.Text;
using Protocolo;

namespace Cliente
{
    class Cliente
    {
        static void Main(string[] args)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0));
            socket.Connect("127.0.0.1", 20000);
            var connected = true;
            Console.WriteLine("Bienvenido al Sistema Client");
            PrintMenu();
            while (connected)
            {
                var opcion = Console.ReadLine();
                switch (opcion)
                {
                    case "exit":
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                        connected = false;
                        break;
                    case "1":
                        Console.WriteLine("Ingrese su nombre de usuario:");
                        var nomUsuario = Console.ReadLine();

                        Console.WriteLine("Ingrese su nombre real:");
                        var nombReal = Console.ReadLine();

                        Console.WriteLine("Ingrese su contraseña:");
                        var contraseña = Console.ReadLine();

                        //Console.WriteLine("Ingrese su foto de perfil:"); FALTA IMPLEMENTAR!

                        var infoUsuario = $"{nomUsuario}?{nombReal}?{contraseña}";

                        EnviarDatos(infoUsuario, socket, CommandConstants.Registro);
                        break;
                    case "2":
                        Console.WriteLine("Ingrese su nombre de usuario:");
                        var nombreLogin = Console.ReadLine();

                        Console.WriteLine("Ingrese su contraseña:");
                        var contraseñaLogin = Console.ReadLine();

                        var infoLogin = $"{nombreLogin}?{contraseñaLogin}";
                        EnviarDatos(infoLogin, socket, CommandConstants.Login);
                        break;
                    case "3":
                        Console.WriteLine("Ingrese el mensaje a enviar:");
                        var mensaje = Console.ReadLine();
                        EnviarDatos(mensaje, socket, CommandConstants.Message);
                        break;
                    default:
                        Console.WriteLine("Opcion invalida");
                        break;
                }
            }

            Console.WriteLine("Exiting Application");
        }
        private static void EnviarDatos(string mensaje, Socket socket, int constante)
        {
            var header = new Header(HeaderConstants.Request, constante, mensaje.Length);
            var data = header.GetRequest();
            var sentBytes = 0;
            while (sentBytes < data.Length)
            {
                sentBytes += socket.Send(data, sentBytes, data.Length - sentBytes, SocketFlags.None);
            }

            sentBytes = 0;
            var bytesMessage = Encoding.UTF8.GetBytes(mensaje);
            while (sentBytes < bytesMessage.Length)
            {
                sentBytes += socket.Send(bytesMessage, sentBytes, bytesMessage.Length - sentBytes,
                    SocketFlags.None);
            }
        }

        private static void PrintMenu()
        {
            Console.WriteLine("Opciones validas: ");
            Console.WriteLine("1 -> registrar un usuario");
            Console.WriteLine("2 -> ingresar al sistema");
            Console.WriteLine("3 -> envia un mensaje al server");
            Console.WriteLine("4 -> lista de usuario");
            Console.WriteLine("exit -> abandonar el programa");
            Console.WriteLine("Ingrese su opcion: ");
        }
    }
}
