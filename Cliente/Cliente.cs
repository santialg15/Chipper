﻿using System.Net;
using System.Net.Sockets;
using System.Text;
using cliente;
using ProyectoCompartido.Interfaces;
using ProyectoCompartido.Protocolo;
using ProyectoCompartido.Protocolo.FileTransfer.FileHandler;
using ProyectoCompartido.Protocolo.FileTransfer.FileHandler.Interfaces;

namespace Cliente
{
    class Cliente
    {
        static bool connected = false;
        static NetworkDataHelper networkDataHelper;
        static string usuLogin = String.Empty;
        static string usuAResponder = String.Empty;

        static TcpClient tcpClient;
        static readonly ISettingsManager SettingsMgr = new SettingsManager();

        static async Task Main(string[] args)
        {
            //Cliente
            var clientIp = IPAddress.Parse(SettingsMgr.ReadSetting(ClientConf.ClientIpConfigKey));
            var clientPort = Int32.Parse(SettingsMgr.ReadSetting(ClientConf.ClientPortConfigKey));
            var ipClientEndpoint = new IPEndPoint(clientIp, clientPort);
            tcpClient = new TcpClient(ipClientEndpoint);

            //Servidor
            var ipServerEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"),20000);

            //Cliente se conecta al servidor
            await tcpClient.ConnectAsync(ipServerEndpoint);
            connected = true;
            
            await using (var stream = tcpClient.GetStream())
            {

                Console.WriteLine("Bienvenido al Sistema Client");
                PrintMenu();
                var task = Task.Run(async () => await HandleServer(networkDataHelper));
                networkDataHelper = new NetworkDataHelper(stream);
                while (connected)
                {
                    var opcion = Console.ReadLine();
                    if (connected)
                    {
                        if (usuLogin.Equals(""))
                        {
                            switch (opcion)
                            {
                                case "exit":
                                    await networkDataHelper.SendMessage("", CommandConstants.exit);
                                    stream.Close();
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

                                    //Console.WriteLine("Ingrese su foto de perfil:"); 

                                    var infoUsuario = $"{nomUsuario}?{nombReal}?{contraseña}";

                                    //try
                                    //{
                                        await networkDataHelper.SendMessage(infoUsuario, CommandConstants.Registro);
                                    //}
                                    //catch (SocketException)
                                    //{
                                    //    Console.WriteLine("Connection with the server has been interrupted");
                                    //    break;
                                    //}

                                    break;
                                case "2":
                                    Console.WriteLine("Ingrese su nombre de usuario:");
                                    var nombreLogin = Console.ReadLine();

                                    Console.WriteLine("Ingrese su contraseña:");
                                    var contraseñaLogin = Console.ReadLine();

                                    var infoLogin = $"{nombreLogin}?{contraseñaLogin}";
                                    usuLogin = nombreLogin;
                                    //try
                                    //{
                                        await networkDataHelper.SendMessage(infoLogin, CommandConstants.Login);
                                    //}
                                    //catch (SocketException)
                                    //{
                                    //    Console.WriteLine("Connection with the server has been interrupted");
                                    //    break;
                                    //}

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
                                    stream.Close();
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
                                            //try
                                            //{
                                                await networkDataHelper.SendMessage(caracteres, CommandConstants.BusquedaIncluyente);
                                            //}
                                            //catch (SocketException)
                                            //{
                                            //   Console.WriteLine("Connection with the server has been interrupted");
                                            //    break;
                                            //}

                                            break;
                                        case "2": //busqueda excluyente
                                            //try
                                            //{
                                                await networkDataHelper.SendMessage(caracteres, CommandConstants.BusquedaExcluyente);
                                            //}
                                            //catch (SocketException)
                                            //{
                                            //    Console.WriteLine("Connection with the server has been interrupted");
                                            //    break;
                                            //}

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
                                    await networkDataHelper.SendMessage(datosParaSeguirUsuario, CommandConstants.SeguirUsuario);
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

                                    if (chip.Length >
                                        Int32.Parse(SettingsMgr.ReadSetting(ClientConf.clientCntCarPubConfigKey)))
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

                                            Console.WriteLine(
                                                "Ingrese las rutas de acceso de las imagenes separadas por ?");
                                            var isValidEnvio = false;
                                            bool isOkControles;
                                            while (!isValidEnvio)
                                            {
                                                isOkControles = true;

                                                var rutasImg = Console.ReadLine();
                                                if (rutasImg.Equals(""))
                                                {
                                                    Console.WriteLine("La ruta no puede ser vacía");
                                                    break;
                                                }

                                                rutasImg += '?';
                                                var dSeparados = rutasImg.Split("?");

                                                if (dSeparados.Length > Int32.Parse(SettingsMgr.ReadSetting(ClientConf.clientCntImgPubConfigKey)) + 1)
                                                {
                                                    Console.WriteLine("Puede ingresar un máximo de " + SettingsMgr.ReadSetting(ClientConf.clientCntImgPubConfigKey) + " archivos. Ingrese las rutas nuevamente");
                                                    isOkControles = false;
                                                }

                                                if (isOkControles)
                                                {
                                                    if (dSeparados.Length == 0)
                                                    {
                                                        Console.WriteLine("Error debe ingresar la ruta de los archivos");
                                                        isOkControles = false;
                                                    }
                                                }

                                                if (isOkControles)
                                                {
                                                    int index = 0;
                                                    IFileHandler fileHandler = new FileHandler();
                                                    while (index < dSeparados.Length && isOkControles)
                                                    {
                                                        string path = dSeparados[index];

                                                        if (!path.Equals("") && !fileHandler.FileExists(path))
                                                        {
                                                            Console.WriteLine("la ruta de acceso al archivo " + index + 1 + " no es valida. Intente nuevamente");
                                                            isOkControles = false;
                                                        }
                                                        index++;
                                                    }
                                                }

                                                if (isOkControles)
                                                {
                                                    await networkDataHelper.SendMessage(usuLogin + "?" + dSeparados.Length
                                                        + "?" + chip, CommandConstants.chip);
                                                    var ClientHandler = new ClientFileHandler();
                                                    ClientHandler.StartServer();

                                                    int index = 0;
                                                    while (index < dSeparados.Length)
                                                    {
                                                        string path = dSeparados[index];
                                                        if (!path.Equals(""))
                                                        {
                                                            ClientHandler.SendFile(path);
                                                        }
                                                        index++;
                                                    }
                                                    ClientHandler.stop();
                                                    isValidEnvio = true;
                                                }
                                            }
                                            break;

                                        case "2":
                                            await networkDataHelper.SendMessage(usuLogin + "?0?" + chip, CommandConstants.chip);
                                            break;
                                        default:
                                            Console.WriteLine("Opcion invalida");
                                            break;
                                    }

                                    PrintLoggedMenu();
                                    break;

                                case "7": //VER Noficaciones
                                    Console.WriteLine("Notificaciones:");
                                    await networkDataHelper.SendMessage(usuLogin, CommandConstants.verNotif);
                                    break;
                                case "8": //VER CHIPS DE UN USUARIO
                                    Console.WriteLine("Ingrese el nombre de usuario:");
                                    var nombreUsuario = Console.ReadLine();
                                    await networkDataHelper.SendMessage(nombreUsuario, CommandConstants.VerChips);
                                    //estaRespondiendoChip = true;
                                    break;

                                case string s
                                    when s.StartsWith("R"): //RECIBE R + NUMERO DE CHIP A CONTESTAR A usuAResponder
                                    var numeroChip = opcion.Substring(1);
                                    if (!int.TryParse(numeroChip, out int num))
                                    {
                                        Console.WriteLine("Debe escribir el numero de chip seguido de la R");
                                        PrintLoggedMenu();
                                        break;
                                    }

                                    Console.WriteLine("Ingrese su respuesta:");
                                    var respuesta = Console.ReadLine();
                                    var datosResponder = $"{usuLogin}?{usuAResponder}?{respuesta}?{numeroChip}";
                                    usuAResponder = "";
                                    await networkDataHelper.SendMessage(datosResponder, CommandConstants.ResponderChip);
                                    break;
                                case "NO":
                                    PrintLoggedMenu();
                                    break;

                                default:
                                    Console.WriteLine("Opcion invalida");
                                    PrintLoggedMenu();
                                    break;
                            }
                        }
                    }
                }
                tcpClient.Close();
            }
        }

        static void PrintMenu()
        {
            Console.WriteLine("Opciones validas: ");
            Console.WriteLine("1 -> Registrar un usuario");
            Console.WriteLine("2 -> Ingresar al sistema");
            Console.WriteLine("exit -> Abandonar el programa");
            Console.WriteLine("Ingrese su opcion: ");
        }

        static void PrintLoggedMenu()
        {
            Console.WriteLine("Menu:");
            Console.WriteLine("4 -> Buscar usuarios");
            Console.WriteLine("5 -> Seguir usuario");
            Console.WriteLine("6 -> Nuevo Chip");
            Console.WriteLine("7 -> Ver mis notificaciones");
            Console.WriteLine("8  -> Ver chips de un usuario");
            Console.WriteLine("exit -> abandonar el programa");
            Console.WriteLine("Ingrese su opcion: ");
        }

        static async Task HandleServer(NetworkDataHelper networkDataHelper)
        {
            while (connected)
            {
                var headerLength = HeaderConstants.Request.Length + HeaderConstants.CommandLength +
                                   HeaderConstants.DataLength;
                var buffer = new byte[headerLength];
                //try
                //{
                    await networkDataHelper.ReceiveData(headerLength, buffer, connected);
                    var header = new Header();
                    connected = header.DecodeData(buffer);
                    switch (header.ICommand)
                    {
                        case CommandConstants.Registro:
                            Console.WriteLine("El servidor esta validando el registro del usuario en el sistema...");
                            var datosRegistro = new byte[header.IDataLength];
                            await networkDataHelper.ReceiveData(header.IDataLength, datosRegistro, connected);
                            var respuestaRegistro = Encoding.UTF8.GetString(datosRegistro);
                            Console.WriteLine($"{respuestaRegistro}");
                            PrintMenu();
                            break;

                        case CommandConstants.Login:
                            Console.WriteLine("El servidor esta validando el ingreso del usuario al sistema...");
                            var datosLogin = new byte[header.IDataLength];
                            await networkDataHelper.ReceiveData(header.IDataLength, datosLogin, connected);
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
                            await networkDataHelper.ReceiveData(header.IDataLength, bufferBusquedaIncluyentes, connected);
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
                            await networkDataHelper.ReceiveData(header.IDataLength, bufferBusquedaExcluyente, connected);
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
                            await networkDataHelper.ReceiveData(header.IDataLength, bufferSeguirUsuario, connected);
                            var respuestaSeguirUsuario = Encoding.UTF8.GetString(bufferSeguirUsuario);
                            if (respuestaSeguirUsuario == "")
                            {
                                Console.WriteLine("El usuario no existe.");
                            }
                            else if (respuestaSeguirUsuario == "Ya sigue a este usuario.")
                            {
                                Console.WriteLine("Ya sigue a este usuario.");
                            }
                            else if(respuestaSeguirUsuario == "No puedes seguirte a ti mismo.")
                            {
                                Console.WriteLine("No puedes seguirte a ti mismo.");
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
                            await networkDataHelper.ReceiveData(header.IDataLength, bufferVerChips, connected);
                            var totalChips = Encoding.UTF8.GetString(bufferVerChips);
                            if (totalChips == "El usuario no existe")
                            {
                                Console.WriteLine("El usuario ingresado no existe");
                                PrintLoggedMenu();
                            }
                            else if (totalChips == "")
                            {
                                Console.WriteLine("El usuario ingresado no tiene chips aun.");
                                PrintLoggedMenu();
                            }
                            else
                            {
                                Console.WriteLine("Lista de chips:");
                                var listaChips = totalChips.Split("?");
                                string usuarioDelChip = "";
                                for (int i = 0; i < listaChips.Length; i++)
                                {
                                    if (i == 0)
                                    {
                                        usuarioDelChip = listaChips[i];
                                        Console.WriteLine(listaChips[i]);
                                    }
                                    else
                                    {
                                        Console.WriteLine($"{listaChips[i]}");
                                    }
                                }
                                usuAResponder = usuarioDelChip;
                                Console.WriteLine("Ingrese R + numero de chip al que quiere responder, de lo contrario escriba: NO.");
                            }
                            break;

                        case CommandConstants.ResponderChip:
                            var bufferResponderChip = new byte[header.IDataLength];
                            await networkDataHelper.ReceiveData(header.IDataLength, bufferResponderChip, connected);
                            var respuestaServidor = Encoding.UTF8.GetString(bufferResponderChip);
                            Console.WriteLine(respuestaServidor);
                            PrintLoggedMenu();
                            break;

                        case CommandConstants.verNotif:
                            var bufferVerNotif = new byte[header.IDataLength];
                            await networkDataHelper.ReceiveData(header.IDataLength, bufferVerNotif, connected);
                            var totalNotif = Encoding.UTF8.GetString(bufferVerNotif);
                            Console.WriteLine("Lista de Notificaciones:");
                            if (totalNotif.Length > 0)
                            {
                                var listaNotif = totalNotif.Split("?");
                                for (int i = 0; i < listaNotif.Length; i++)
                                {
                                    Console.WriteLine(listaNotif[i].ToString());
                                }
                            }
                            else
                            {
                                Console.WriteLine("No tiene notificaciones");
                            }
                            PrintLoggedMenu();
                            break;
                    }
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine($"Server is closing, will not process more data -> Message {e.Message}..");
                //    connected = false;
                //}
            }
            //tcpClient.Close();
        }
        }
    }
