using System.Net;
using System.Net.Sockets;
using System.Text;
using Protocolo;
using Protocolo.Interfaces;

namespace Cliente
{
    class Cliente
    {
        static readonly ISettingsManager SettingsMgr = new SettingsManager();
        static void Main(string[] args)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0));
            socket.Connect("127.0.0.1", 20000);
            var connected = true;
            NetworkDataHelper networkDataHelper = new NetworkDataHelper(socket);
            Console.WriteLine("Bienvenido al Sistema Cliente");
            string UsuLogin = "12345"; //pNomUsu
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

                        networkDataHelper.EnviarDatos(infoUsuario, socket, CommandConstants.Registro);
                        break;
                    case "2":
                        Console.WriteLine("Ingrese su nombre de usuario:");
                        var nombreLogin = Console.ReadLine();

                        Console.WriteLine("Ingrese su contraseña:");
                        var contraseñaLogin = Console.ReadLine();

                        var infoLogin = $"{nombreLogin}?{contraseñaLogin}";
                        networkDataHelper.EnviarDatos(infoLogin, socket, CommandConstants.Login);
                        break;
                    case "3":
                        Console.WriteLine("Ingrese el mensaje a enviar:");
                        var mensaje = Console.ReadLine();
                        networkDataHelper.EnviarDatos(mensaje, socket, CommandConstants.Message);
                        break;
                    case "6":
                        Console.WriteLine("Chip:");
                        var chip = Console.ReadLine();

                        if (chip.Length == 0)
                        {
                            Console.WriteLine("Un chip no puede ser vacío");
                            PrintMenu();
                            break;
                        }
                        if (chip.Length > Int32.Parse(SettingsMgr.ReadSetting(ClientConf.clientCntCarPubConfigKey)))
                        {
                            Console.WriteLine("Un chip puede tener un máximo de "+ SettingsMgr.ReadSetting(ClientConf.clientCntCarPubConfigKey) +" caracteres");
                            PrintMenu();
                            break;
                        }

                        Console.WriteLine("Ingresa imagenes?:");
                        Console.WriteLine("1 -> Si");
                        Console.WriteLine("2 -> No");

                        var op = Console.ReadLine();
                        switch (op)
                        {
                            case "1":
                                Console.WriteLine("Ingrese las rutas de acceso de las imagenes separadas por ?");
                                var rutasImg = Console.ReadLine();
                                // enviar img
                                break;
                            case "2":

                                networkDataHelper.EnviarDatos(UsuLogin+ "?"+chip, socket, CommandConstants.chip);
                                break;
                            default:
                            Console.WriteLine("Opcion invalida");
                            PrintMenu();
                            break;
                        }
                        break;
                    default:
                        Console.WriteLine("Opcion invalida");
                        PrintMenu();
                        break;
                }
            }

            Console.WriteLine("Exiting Application");
        }

        private static void PrintMenu()
        {
            Console.WriteLine("Opciones validas: ");
            Console.WriteLine("1 -> Registrar un usuario");
            Console.WriteLine("2 -> Ingresar al sistema");
            Console.WriteLine("3 -> Envia un mensaje al server");
            Console.WriteLine("4 -> Lista de usuario");
            Console.WriteLine("5 -> Seguir usuario");
            Console.WriteLine("6 -> Nuevo chip");
            Console.WriteLine("exit -> Abandonar el programa");
            Console.WriteLine("Ingrese su opcion: ");
        }
    }
}
