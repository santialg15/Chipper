using System.Net;
using System.Net.Sockets;
using System.Text;
using Protocolo;
using Protocolo.Interfaces;

namespace Cliente
{
    class Cliente
    {
        static bool connected = false;
        static NetworkDataHelper networkDataHelper;

        static readonly ISettingsManager SettingsMgr = new SettingsManager();
        static void Main(string[] args)
        {
            var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0));
            clientSocket.Connect("127.0.0.1", 20000);
            connected = true;
            NetworkDataHelper networkDataHelper = new NetworkDataHelper(clientSocket);
            Console.WriteLine("Bienvenido al Sistema Client");
            string UsuLogin = "12345"; //pNomUsu
            PrintMenu();

            new Thread(() => HandleServer(clientSocket,networkDataHelper)).Start();

            while (connected)
            {
                var opcion = Console.ReadLine();
                switch (opcion)
                {
                    case "exit":
                        clientSocket.Shutdown(SocketShutdown.Both);
                        clientSocket.Close();
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

                        try
                        {
                            networkDataHelper.SendMessage(clientSocket, infoUsuario,CommandConstants.Registro);
                        }
                        catch (SocketException)
                        {
                            Console.WriteLine("Connection with the server has been interrupted");
                            break;
                        }
                        break;
                    case "2":
                        Console.WriteLine("Ingrese su nombre de usuario:");
                        var nombreLogin = Console.ReadLine();

                        Console.WriteLine("Ingrese su contraseña:");
                        var contraseñaLogin = Console.ReadLine();

                        var infoLogin = $"{nombreLogin}?{contraseñaLogin}";
                        try
                        {
                            networkDataHelper.SendMessage(clientSocket, infoLogin, CommandConstants.Login);
                        }
                        catch (SocketException)
                        {
                            Console.WriteLine("Connection with the server has been interrupted");
                            break;
                        }
                        break;
                    case "3":
                        Console.WriteLine("Ingrese el mensaje a enviar:");
                        var mensaje = Console.ReadLine();
                        try
                        {
                            networkDataHelper.SendMessage(clientSocket, mensaje, CommandConstants.Message);
                        }
                        catch (SocketException)
                        {
                            Console.WriteLine("Connection with the server has been interrupted");
                            break;
                        }
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

        private static void PrintLoggedMenu()
        {
            Console.WriteLine("Menu:");
            Console.WriteLine("1 -> Buscar usuarios");
            Console.WriteLine("2 -> Seguir usuario");
            Console.WriteLine("3 -> Crear una publicacion");
            Console.WriteLine("4 -> Ver perfil");
            Console.WriteLine("4 -> Ver mi perfil");
            Console.WriteLine("5 -> Ver mis publicaciones");
            Console.WriteLine("exit -> abandonar el programa");
            Console.WriteLine("Ingrese su opcion: ");
        }

        private static void HandleServer(Socket clientSocket, NetworkDataHelper networkDataHelper)
        {
            while (connected)
            {
                var headerLength = HeaderConstants.Request.Length + HeaderConstants.CommandLength +
                                   HeaderConstants.DataLength;
                var buffer = new byte[headerLength];
                try
                {
                    networkDataHelper.ReceiveData(clientSocket, headerLength, buffer, connected);
                    var header = new Header();
                    header.DecodeData(buffer);
                    switch (header.ICommand)
                    {
                        case CommandConstants.Registro:
                            Console.WriteLine("El servidor esta validando el registro del usuario en el sistema...");
                            var datosRegistro = new byte[header.IDataLength];
                            networkDataHelper.ReceiveData(clientSocket, header.IDataLength, datosRegistro, connected);
                            var respuestaRegistro = Encoding.UTF8.GetString(datosRegistro);
                            Console.WriteLine($"{respuestaRegistro}");
                            PrintMenu();
                            break;
                        case CommandConstants.Login:
                            Console.WriteLine("El servidor esta validando el ingreso del usuario al sistema...");
                            var datosLogin = new byte[header.IDataLength];
                            networkDataHelper.ReceiveData(clientSocket, header.IDataLength, datosLogin, connected);
                            var respuestaLogin = Encoding.UTF8.GetString(datosLogin);
                            Console.WriteLine($"{respuestaLogin}");
                            if(respuestaLogin == "El usuario se logueo correctamente.") 
                            {
                                PrintLoggedMenu();
                                break;
                            }
                            PrintMenu();
                            break;
                        //case CommandConstants.ListUsers:
                        //    for (int i = 0; i < _usuarios.Count; i++)
                        //    {
                        //        Console.WriteLine(_usuarios[i].ToString());

                        //    }
                        //    break;
                        case CommandConstants.Message:
                            Console.WriteLine("El servidor esta contestando...");
                            var bufferData = new byte[header.IDataLength];
                            networkDataHelper.ReceiveData(clientSocket, header.IDataLength, bufferData, connected);
                            Console.WriteLine("Message received: " + Encoding.UTF8.GetString(bufferData));
                            PrintMenu();
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Server is closing, will not process more data -> Message {e.Message}..");
                }
            }
        }
    }
}
