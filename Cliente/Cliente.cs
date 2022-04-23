﻿using System.Net;
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
                                    networkDataHelper.SendMessage(clientSocket, caracteres, CommandConstants.BusquedaIncluyente);
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
                                    networkDataHelper.SendMessage(clientSocket, caracteres, CommandConstants.BusquedaExcluyente);
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

                        networkDataHelper.EnviarDatos(UsuLogin + "?"+ op + "?" + chip, socket, CommandConstants.chip);
                        switch (op)
                        {
                            case "1": //ingresa img 
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
                                    PrintMenu();
                                }
                                PrintMenu();

                                break;
                            default:
                            Console.WriteLine("Opcion invalida");
                            PrintMenu();
                            break;
                        }
                        break;

                    case "8": //VER CHIPS DE UN USUARIO
                        Console.WriteLine("Ingrese el nombre de usuario:");
                        var nombreUsuario = Console.ReadLine();
                        networkDataHelper.SendMessage(clientSocket, nombreUsuario, CommandConstants.VerChips);
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
                            if(respuestaLogin == "El usuario se logueo correctamente.") 
                            {
                                PrintLoggedMenu();
                                break;
                            }
                            PrintMenu();
                            break;
                        case CommandConstants.Message:
                            Console.WriteLine("El servidor esta contestando...");
                            var bufferData = new byte[header.IDataLength];
                            networkDataHelper.ReceiveData(clientSocket, header.IDataLength, bufferData, connected);
                            Console.WriteLine("Message received: " + Encoding.UTF8.GetString(bufferData));
                            PrintMenu();
                            break;
                        case CommandConstants.BusquedaIncluyente:
                            Console.WriteLine("El servidor esta validando la busqueda...");
                            var bufferBusquedaIncluyentes = new byte[header.IDataLength];
                            networkDataHelper.ReceiveData(clientSocket, header.IDataLength, bufferBusquedaIncluyentes, connected);
                            var totalUsuarios = Encoding.UTF8.GetString(bufferBusquedaIncluyentes);
                            if(totalUsuarios == "")
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
                            networkDataHelper.ReceiveData(clientSocket, header.IDataLength, bufferBusquedaExcluyente, connected);
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
                        case CommandConstants.VerChips:
                            Console.WriteLine("El servidor esta validando los chips del usuario ingresado...");
                            var bufferVerChips = new byte[header.IDataLength];
                            networkDataHelper.ReceiveData(clientSocket,header.IDataLength, bufferVerChips, connected);
                            var totalChips = Encoding.UTF8.GetString(bufferVerChips);
                            if(totalChips == "El usuario no existe")
                            {
                                Console.WriteLine("El usuario ingresado no existe");
                            }
                            else if(totalChips == "")
                            {
                                Console.WriteLine("El usuario ingresado no tiene chips aun.");
                            }
                            else 
                            {
                                Console.WriteLine("Lista de chips:");
                                var listaChips = totalChips.Split("?");
                                for (int i = 0; i < listaChips.Length; i++)
                                {
                                    Console.WriteLine(listaChips[i].ToString());
                                }
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
    }
}
