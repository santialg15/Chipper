using System.Net;
using System.Net.Sockets;
using System.Text;
using ConsoleArchiveSender;
using Microsoft.VisualBasic;
using Protocolo;
using Protocolo.FileHandler;
using Protocolo.FileHandler.Interfaces;
using Protocolo.Interfaces;

namespace Cliente
{
    class Cliente
    {
        static bool connected = false;
        static NetworkDataHelper networkDataHelper;
        static string usuLogin = String.Empty;

        static readonly ISettingsManager SettingsMgr = new SettingsManager();

        static void Main(string[] args)
        {
            var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0));
            clientSocket.Connect("127.0.0.1", 20000);
            connected = true;
            NetworkDataHelper networkDataHelper = new NetworkDataHelper(clientSocket);
            Console.WriteLine("Bienvenido al Sistema Client");

            PrintMenu();

            new Thread(() => HandleServer(clientSocket, networkDataHelper)).Start();

            while (connected)
            {
                var opcion = Console.ReadLine();

                if (usuLogin.Equals(""))
                {
                    switch (opcion)
                    {
                        case "exit":
                            clientSocket.Shutdown(SocketShutdown.Both);
                            clientSocket.Close();
                            connected = false;
                            usuLogin = "";
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
                                networkDataHelper.SendMessage(clientSocket, infoUsuario, CommandConstants.Registro);
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
                            usuLogin = nombreLogin;
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
                        default:
                            Console.WriteLine("Opcion invalida");
                            PrintMenu();
                            break;
                    }
                }
                else
                {
                    switch (opcion)
                    {
                        case "exit":
                            clientSocket.Shutdown(SocketShutdown.Both);
                            clientSocket.Close();
                            connected = false;
                            usuLogin = "";
                            break;

                        case "4": //BUSQUEDA DE USUARIOS
                            Console.WriteLine("Ingresar caracteres del usuario a buscar:");
                            var caracteres = Console.ReadLine();
                            Console.WriteLine("Elija el tipo de busqueda:");
                            Console.WriteLine("1 -> Busqueda incluyente");
                            Console.WriteLine("2 -> Busqueda excluyente");
                            var tipoDeBusqueda = Console.ReadLine();
                            switch (tipoDeBusqueda)
                            {
                                case "1": //busqueda incluyente
                                    try
                                    {
                                        networkDataHelper.SendMessage(clientSocket, caracteres,
                                            CommandConstants.BusquedaIncluyente);
                                    }
                                    catch (SocketException)
                                    {
                                        Console.WriteLine("Connection with the server has been interrupted");
                                        break;
                                    }

                                    break;
                                case "2": //busqueda excluyente
                                    try
                                    {
                                        networkDataHelper.SendMessage(clientSocket, caracteres,
                                            CommandConstants.BusquedaExcluyente);
                                    }
                                    catch (SocketException)
                                    {
                                        Console.WriteLine("Connection with the server has been interrupted");
                                        break;
                                    }

                                    break;
                                default:
                                    Console.WriteLine("Opcion invalida");
                                    PrintLoggedMenu();
                                    break;
                            }

                            break;

                        case "5": //SEGUIR A UN USUARIO
                            Console.WriteLine("Ingrese el nombre del usuario a seguir:");
                            var nombreASeguir = Console.ReadLine();
                            var datosParaSeguirUsuario = $"{usuLogin}?{nombreASeguir}";
                            networkDataHelper.SendMessage(clientSocket, datosParaSeguirUsuario, CommandConstants.SeguirUsuario);
                            break;
                        case "6": //NUEVO CHIP
                            Console.WriteLine("Chip:");
                            var chip = Console.ReadLine();

                            if (chip.Length == 0)
                            {
                                Console.WriteLine("Un chip no puede ser vacío");
                                PrintLoggedMenu();
                                break;
                            }

                            if (chip.Length > Int32.Parse(SettingsMgr.ReadSetting(ClientConf.clientCntCarPubConfigKey)))
                            {
                                Console.WriteLine("Un chip puede tener un máximo de " +
                                                  SettingsMgr.ReadSetting(ClientConf.clientCntCarPubConfigKey) +
                                                  " caracteres");
                                PrintLoggedMenu();
                                break;
                            }

                            Console.WriteLine("Ingresa imagenes?:");
                            Console.WriteLine("1 -> Si");
                            Console.WriteLine("2 -> No");

                            var op = Console.ReadLine();

                            switch (op)
                            {
                                case "1": //ingresa img 
                                    networkDataHelper.SendMessage(clientSocket, usuLogin + "?" + op + "?" + chip, CommandConstants.chip);
                                    var ClientHandler = new ClientFileHandler();
                                    ClientHandler.StartServer();
                                    Console.WriteLine("Ingrese las rutas de acceso de las imagenes separadas por ?");
                                    var rutasImg = Console.ReadLine();
                                    rutasImg += '?';
                                    var dSeparados = rutasImg.Split("?");

                                    if (dSeparados.Length > 0)
                                    {
                                        int index = 0;
                                        while (index < dSeparados.Length)
                                        {
                                            string path = dSeparados[index];
                                            IFileHandler fileHandler = new FileHandler();
                                            if (fileHandler.FileExists(path))
                                            {
                                                ClientHandler.SendFile(path);
                                            }

                                            index++;
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error debe ingresar la ruta de los archivos");
                                    }
                                    break;

                                case "2":
                                    networkDataHelper.SendMessage(clientSocket, usuLogin + "?" + op + "?" + chip, CommandConstants.chip);
                                    break;
                                default:
                                    Console.WriteLine("Opcion invalida");
                                    break;

                            }
                            PrintLoggedMenu();
                            break;

                        case "7": //VER Noficaciones
                            Console.WriteLine("Notificaciones:");
                            networkDataHelper.SendMessage(clientSocket, usuLogin, CommandConstants.verNotif);
                            break;
                        case "8": //VER CHIPS DE UN USUARIO
                            Console.WriteLine("Ingrese el nombre de usuario:");
                            var nombreUsuario = Console.ReadLine();
                            networkDataHelper.SendMessage(clientSocket, nombreUsuario, CommandConstants.VerChips);
                            break;
                        default:
                            Console.WriteLine("Opcion invalida");
                            PrintLoggedMenu();
                            break;
                    }
                }
            }

            Console.WriteLine("Exiting Application");
        }

        private static void PrintMenu()
        {
            Console.WriteLine("Opciones validas: ");
            Console.WriteLine("1 -> Registrar un usuario");
            Console.WriteLine("2 -> Ingresar al sistema");
            Console.WriteLine("exit -> Abandonar el programa");
            Console.WriteLine("Ingrese su opcion: ");
        }

        private static void PrintLoggedMenu()
        {
            Console.WriteLine("Menu:");
            Console.WriteLine("4 -> Buscar usuarios");
            Console.WriteLine("5 -> Seguir usuario");
            Console.WriteLine("6 -> Nuevo Chip");
            Console.WriteLine("7 -> Ver mis notificaciones");
            Console.WriteLine("8  -> Ver chips de un usuario");
            Console.WriteLine("9 -> Responder un chip");
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
                            if (respuestaLogin == "El usuario se logueo correctamente.")
                            {
                                PrintLoggedMenu();
                                break;
                            }
                            else
                            {
                                usuLogin = "";
                                PrintMenu();
                            }

                            break;

                        case CommandConstants.BusquedaIncluyente:
                            Console.WriteLine("El servidor esta validando la busqueda...");
                            var bufferBusquedaIncluyentes = new byte[header.IDataLength];
                            networkDataHelper.ReceiveData(clientSocket, header.IDataLength, bufferBusquedaIncluyentes,
                                connected);
                            var totalUsuarios = Encoding.UTF8.GetString(bufferBusquedaIncluyentes);
                            if (totalUsuarios == "")
                            {
                                Console.WriteLine("No se encontraron usuarios");
                            }
                            else
                            {
                                var listaUsuarios = totalUsuarios.Split("?");
                                Console.WriteLine("Lista de usuarios solicitada:");
                                for (int i = 0; i < listaUsuarios.Length; i++)
                                {
                                    Console.WriteLine($"{listaUsuarios[i]}");
                                }
                            }

                            PrintLoggedMenu();
                            break;

                        case CommandConstants.BusquedaExcluyente:
                            Console.WriteLine("El servidor esta validando la busqueda...");
                            var bufferBusquedaExcluyente = new byte[header.IDataLength];
                            networkDataHelper.ReceiveData(clientSocket, header.IDataLength, bufferBusquedaExcluyente,
                                connected);
                            var totalUsuariosExcluyentes = Encoding.UTF8.GetString(bufferBusquedaExcluyente);
                            if (totalUsuariosExcluyentes == "")
                            {
                                Console.WriteLine("No se encontraron usuarios");
                            }
                            else
                            {
                                var listaUsuarios = totalUsuariosExcluyentes.Split("?");
                                Console.WriteLine("Lista de usuarios solicitada:");
                                for (int i = 0; i < listaUsuarios.Length; i++)
                                {
                                    Console.WriteLine($"{listaUsuarios[i]}");
                                }
                            }

                            PrintLoggedMenu();
                            break;

                        case CommandConstants.SeguirUsuario:
                            Console.WriteLine("El servidor esta validando el seguimiento de usuario...");
                            var bufferSeguirUsuario = new byte[header.IDataLength];
                            networkDataHelper.ReceiveData(clientSocket, header.IDataLength, bufferSeguirUsuario, connected);
                            var respuestaSeguirUsuario = Encoding.UTF8.GetString(bufferSeguirUsuario);
                            if (respuestaSeguirUsuario == "")
                            {
                                Console.WriteLine("El usuario no existe.");
                            }
                            else if(respuestaSeguirUsuario == "Ya sigue a este usuario.")
                            {
                                Console.WriteLine("Ya sigue a este usuario.");
                            }
                            else
                            {
                                Console.WriteLine("El usuario fue seguido correctamente.");
                            }
                            PrintLoggedMenu();
                            break;

                        case CommandConstants.VerChips:
                            Console.WriteLine("El servidor esta validando los chips del usuario ingresado...");
                            var bufferVerChips = new byte[header.IDataLength];
                            networkDataHelper.ReceiveData(clientSocket, header.IDataLength, bufferVerChips, connected);
                            var totalChips = Encoding.UTF8.GetString(bufferVerChips);
                            if (totalChips == "El usuario no existe")
                            {
                                Console.WriteLine("El usuario ingresado no existe");
                            }
                            else if (totalChips == "")
                            {
                                Console.WriteLine("El usuario ingresado no tiene chips aun.");
                            }
                            else
                            {
                                Console.WriteLine("Lista de chips:");
                                var listaChips = totalChips.Split("?");
                                string usuarioDelChip = "";
                                for (int i = 0; i < listaChips.Length-1; i++)
                                {
                                    if(i == 0) 
                                    {
                                        usuarioDelChip = listaChips[i];
                                        Console.WriteLine(listaChips[i]);
                                    }
                                    else
                                    {
                                        Console.WriteLine($"{listaChips[i]}");
                                    }
                                }
                                //EMPIEZA FUNCIONALIDAD CRF9 RESPONDER CHIP
                                Console.WriteLine("Ingrese el numero de chip al que quiere responder, de lo contrario escriba: NO.");
                                var decision = Console.ReadLine();
                                if(decision == "NO" || decision == "no")
                                {
                                    PrintLoggedMenu();
                                }
                                else
                                {
                                    if (int.TryParse(decision, out int num))
                                    {
                                        ResponderUnChip(clientSocket, header, usuLogin, usuarioDelChip, decision);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Debe ingresar un numero correcto.");
                                    }
                                }
                            }
                            PrintLoggedMenu();
                            break;

                        case CommandConstants.ResponderChip:
                            var bufferResponderChip = new byte[header.IDataLength];
                            networkDataHelper.ReceiveData(clientSocket, header.IDataLength, bufferResponderChip, connected);
                            var respuestaServidor = Encoding.UTF8.GetString(bufferResponderChip);
                            Console.WriteLine(respuestaServidor);
                            PrintLoggedMenu();
                            break;

                        case CommandConstants.verNotif:
                            var bufferVerNotif = new byte[header.IDataLength];
                            networkDataHelper.ReceiveData(clientSocket, header.IDataLength, bufferVerNotif, connected);
                            var totalNotif = Encoding.UTF8.GetString(bufferVerNotif);
                            Console.WriteLine("Lista de Notificaciones:");
                            if (totalNotif.Length > 0)
                            {
                                var listaNotif = totalNotif.Split("?");
                                for (int i = 0; i < listaNotif.Length; i++)
                                {
                                    Console.WriteLine(listaNotif[i].ToString());
                                }
                            }else
                            {
                                Console.WriteLine("No tiene notificaciones");
                            }
                            PrintLoggedMenu();
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Server is closing, will not process more data -> Message {e.Message}..");
                }
            }
        }

        private static void ResponderUnChip(Socket clientSocket, Header header, string usuarioLogueado, string usuarioDelChip, string numeroChip)
        {
            Console.WriteLine("Escriba la respuesta:");
            var respuesta = Console.ReadLine();
            if(respuesta == "")
            {
                Console.WriteLine("La respuesta no puede ser vacia.");
                PrintLoggedMenu();
            }
            else
            {
                var datosRespuestaChip = $"{usuarioLogueado}?{usuarioDelChip}?{numeroChip}?{respuesta}";
                networkDataHelper.SendMessage(clientSocket, datosRespuestaChip, CommandConstants.ResponderChip);
            }
        }
    }
}