using System.Net;
using System.Net.Sockets;
using System.Text;
using Protocolo;
using Protocolo.Interfaces;


namespace Servidor
{
    internal static class Servidor
    {
        static readonly ISettingsManager SettingsMgr = new SettingsManager();
        static bool _exit = false;
        static List<Socket> _clients = new List<Socket>();
        private static List<Usuario> _usuarios = new List<Usuario>();
        private static NetworkDataHelper networkDataHelper;


        static void Main(string[] args)
        {
            Usuario _usu1 = new Usuario("Denis", "12345","inicio","img");
            Usuario _usu2 = new Usuario("Santiago", "67890", "inicio", "img");
            _usuarios.Add(_usu1);
            _usuarios.Add(_usu2);

            var socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketServer.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 20000));
            socketServer.Listen(100);

            //Lanzar un thread para manejar las conexiones
            var threadServer = new Thread(()=> ListenForConnections(socketServer));
            threadServer.Start();

            Console.WriteLine("Bienvenido al Sistema Server");
            printMenu();
            while (!_exit)
            {
                var userInput = Console.ReadLine();
                switch (userInput)
                {
                    // Cosas a hacer al cerrar el server
                    // 1 - Cerrar el socket que esta escuchando conexiones nuevas
                    // 2 - Cerrar todas las conexiones abiertas desde los clientes
                    case "1": // SRF1
                        _exit = true;
                        socketServer.Close(0);
                        foreach (var client in _clients)
                        {
                            client.Shutdown(SocketShutdown.Both);
                            client.Close();
                        }
                        var fakeSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
                        fakeSocket.Connect("127.0.0.1",20000);
                        break;

                    case "2": // SRF2

                        if (_usuarios.Count == 0)
                        {
                            Console.WriteLine("No hay usuarios registrados en el sistema.");
                            printMenu();
                            break;
                        }

                        foreach (var usu3 in _usuarios)
                        {
                            Console.WriteLine(usu3.ToString());
                        }
                       
                        printMenu();
                        break;

                    case "3": // SRF5
                        Console.WriteLine("Ingrese su búsqueda: ");
                        var buscar = Console.ReadLine();
                        string ret = "";
                        foreach (var usu4 in _usuarios)
                        {
                            foreach (var pub in usu4.GetPublicaciones())
                            {
                                if (pub.getContenido().Contains(buscar.Trim()))
                                {
                                    ret += usu4.getNomUsu() + pub.getContenido() + "\n";
                                }
                            }
                        }

                        if (ret.Equals(""))
                        {
                            Console.WriteLine("No se encontraron chips que contengan: " + buscar.Trim());
                        }
                        printMenu();
                        break;

                    case "4": // SRF6
                        _usuarios.OrderBy(cantSeg => cantSeg.GetCantSeg());
                        int contador = 0;
                        while (contador < Int32.Parse(SettingsMgr.ReadSetting(ServerConfig.SeverTopSeguidoresConfigKey)) && contador < _usuarios.Count)
                        {
                            Console.WriteLine(_usuarios[contador].ToString());
                            contador++;
                        }
                        printMenu();
                        break;

                    case "5":
                        if(_usuarios.Count == 0)
                        {
                            Console.WriteLine("No hay usuarios registrados en el sistema.");
                            printMenu();
                            break;
                        }
                        if (!ExistenUsuariosHabilitados())
                        {
                            Console.WriteLine("No hay usuarios con premiso de acceso al sistema.");
                            printMenu();
                            break;
                        }
                        Console.WriteLine("Lista de usuarios con permiso de acceso:");
                        foreach (var u in _usuarios)
                        {
                            if (u.Habilitado == true)
                                Console.WriteLine(u.ToString());
                        }
                        Console.WriteLine("Ingresar nombre de usuario al que se le quiere negar acceso");
                        var nombreANegar = Console.ReadLine();
                        if(nombreANegar == null || nombreANegar == "")
                        {
                            Console.WriteLine("El usuario ingresado no existe.");
                            printMenu();
                            break;
                        }
                        var indiceNegar = _usuarios.FindIndex(u => u.PNomReal == nombreANegar);
                        if(indiceNegar == -1)
                        {
                            Console.WriteLine($"No existe el usuario {nombreANegar} en el sistema.");
                            printMenu();
                            break;
                        }
                        _usuarios[indiceNegar].Habilitado = false;
                        Console.WriteLine($"Se le nego el acceso al usuario {nombreANegar}.");
                        printMenu();
                        break;

                    case "6":
                        if (_usuarios.Count == 0)
                        {
                            Console.WriteLine("No hay usuarios registrados en el sistema.");
                            printMenu();
                            break;
                        }
                        if (!ExistenUsuariosNoHabilitados())
                        {
                            Console.WriteLine("No hay usuarios con acceso denegado al sistema.");
                            printMenu();
                            break;
                        }
                        Console.WriteLine("Lista de usuarios con acceso denegado:");
                        foreach (var u in _usuarios)
                        {
                            if(u.Habilitado == false)
                                Console.WriteLine(u.ToString());
                        }
                        Console.WriteLine("Ingresar nombre de usuario al que se le quiere permitir el acceso");
                        var nombreAPermitir = Console.ReadLine();
                        if (nombreAPermitir == null || nombreAPermitir == "")
                        {
                            Console.WriteLine("El usuario ingresado no existe.");
                            printMenu();
                            break;
                        }
                        var indicePermitir = _usuarios.FindIndex(u => u.PNomReal == nombreAPermitir);
                        if (indicePermitir == -1)
                        {
                            Console.WriteLine("El usuario ingresado no existe.");
                            printMenu();
                            break;
                        }
                        _usuarios[indicePermitir].Habilitado = true;
                        Console.WriteLine($"Se le permitio el acceso al usuario {nombreAPermitir}.");
                        printMenu();
                        break;
                   
                        case "7": // SRF7
                            _usuarios.OrderBy(usuario => usuario.GetCantPubEnTmpConf());
                            int cont = 0;
                            while (cont < Int32.Parse(SettingsMgr.ReadSetting(ServerConfig.SeverTopMasUsosConfigKey)) && cont < _usuarios.Count)
                            { 
                                Console.WriteLine(_usuarios[cont].ToString());
                                cont++;
                            }
                            printMenu();
                        break;
                    default:
                        Console.WriteLine("Opcion incorrecta ingresada");
                        break;
                }
            }
        }

        private static void printMenu()
        {
            Console.WriteLine("1 -> abandonar el programa");
            Console.WriteLine("2 -> listar usuarios");
            Console.WriteLine("3 -> Buscar chips");
            Console.WriteLine("4 -> top " + SettingsMgr.ReadSetting(ServerConfig.SeverTopSeguidoresConfigKey) + " con más seguidores");
            Console.WriteLine("5 -> Negar acceso a usuario");
            Console.WriteLine("6 -> Permitir acceso a usuario");
            Console.WriteLine("7 -> top " + SettingsMgr.ReadSetting(ServerConfig.SeverTopMasUsosConfigKey) + " que más usaron el sistema en los ultimos " + SettingsMgr.ReadSetting(ServerConfig.SeverTmpMostrarPubConfigKey) +" minutos");
            Console.WriteLine("Ingrese el numero de la opción deseada: ");
        }
        
        private static void ListenForConnections(Socket socketServer)
        {
            while (!_exit)
            {
                try
                {
                    var clientConnected = socketServer.Accept();
                    _clients.Add(clientConnected);

                    networkDataHelper = new NetworkDataHelper(clientConnected);

                    Console.WriteLine("Nueva conexion aceptada...");
                    var threadcClient = new Thread(() => HandleClient(clientConnected));
                    threadcClient.Start();

                    //SendMessage(networkDataHelper);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    _exit = true;
                }
            }
            Console.WriteLine("Exiting....");
        }

        private static void HandleClient(Socket clientSocket)
        {
            while (!_exit)
            {
                var headerLength = HeaderConstants.Request.Length + HeaderConstants.CommandLength +
                                   HeaderConstants.DataLength;
                var buffer = new byte[headerLength];
                try
                {
                    networkDataHelper.ReceiveData(clientSocket, headerLength, buffer, _exit);
                    var header = new Header();
                    header.DecodeData(buffer);

                    switch (header.ICommand)
                    {
                        case CommandConstants.Registro:
                            Console.WriteLine("Validando registro de un usuario en el sistema");
                            var datosRegistro = ObtenerDatosDelCliente(header, clientSocket);
                            var datosSeparados = datosRegistro.Split("?");
                            var nombreUsuario = datosSeparados[0];
                            var nombreReal = datosSeparados[1];
                            var contraseña = datosSeparados[2];
                            ValidarRegistroUsuario(clientSocket, nombreUsuario, nombreReal, contraseña);
                            break;

                        case CommandConstants.Login:
                            Console.WriteLine("Validando ingreso de usuario en el sistema");
                            var datosLogin = ObtenerDatosDelCliente(header, clientSocket);
                            var datosLoginSeparados = datosLogin.Split("?");
                            var nombreLogin = datosLoginSeparados[0];
                            var contraseñaLogin = datosLoginSeparados[1];
                            ValidarLoginUsuario(networkDataHelper, clientSocket, nombreLogin, contraseñaLogin);
                            break;

                        case CommandConstants.BusquedaIncluyente:
                            Console.WriteLine("El usuario inicio una busqueda de usuarios...");
                            var datosBusquedaIncluyente = ObtenerDatosDelCliente(header, clientSocket);
                            BusquedaUsuarios(clientSocket, datosBusquedaIncluyente, CommandConstants.BusquedaIncluyente);
                            break;

                        case CommandConstants.BusquedaExcluyente:
                            Console.WriteLine("El usuario inicio una busqueda de usuarios...");
                            var datosBusquedaExcluyente = ObtenerDatosDelCliente(header, clientSocket);
                            BusquedaUsuarios(clientSocket, datosBusquedaExcluyente, CommandConstants.BusquedaExcluyente);
                            break;

                        case CommandConstants.Message:
                            Console.WriteLine("Will receive message to display...");
                            var bufferData = new byte[header.IDataLength];
                            networkDataHelper.ReceiveData(clientSocket, header.IDataLength, bufferData, _exit);
                            Console.WriteLine("Message received: " + Encoding.UTF8.GetString(bufferData));
                            networkDataHelper.SendMessage(clientSocket, "El mensaje fue recibido correctamente!.",CommandConstants.Message);
                            break;

                        case CommandConstants.chip:
                            var dLogin = ObtenerDatosDelCliente(header, clientSocket);
                            var dSeparados = dLogin.Split("?");
                            var nomUsu = dSeparados[0];
                            var chip = dSeparados[1];
                            Usuario usuChip = buscarUsuarioLogin(nomUsu);
                            usuChip.nuevoChip(chip);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Server is closing, will not process more data -> Message {e.Message}..");
                }
            }
        }

        //Devuelve el usuario logueado a partir de su nombre de usuario
        private static Usuario buscarUsuarioLogin(string usuLogin)
        {
            foreach (var usu in _usuarios)
            {
                if (usu.getNomUsu().Equals(usuLogin))
                {
                    return usu;
                }
            }

            return null;
        }


        private static string ObtenerDatosDelCliente(Header header, Socket clientSocket)
        {
            var datosBuffer = new byte[header.IDataLength];
            networkDataHelper.ReceiveData(clientSocket, header.IDataLength, datosBuffer, _exit);
            return Encoding.UTF8.GetString(datosBuffer);
        }

        private static void ValidarRegistroUsuario(Socket clientSocket, string nombreUsuario, string nombreReal, string contraseña)
        {
            if (nombreUsuario == "" || nombreReal == "" || contraseña == "")
            {
                networkDataHelper.SendMessage(clientSocket, "Ningun campo puede estar vacio.", CommandConstants.Registro);
                Console.WriteLine("No se realizo el registro de usuario.");
            }
            else if (_usuarios.Exists(u => u.PNomUsu == nombreUsuario))
            {
                networkDataHelper.SendMessage(clientSocket, "El usuario ya existe.", CommandConstants.Registro);
                Console.WriteLine("No se realizo el registro de usuario.");
            }
            else
            {
                Usuario nuevoUsuario = new Usuario(nombreReal, nombreUsuario, contraseña, "imagen");
                _usuarios.Add(nuevoUsuario);
                Console.WriteLine($"Usuario {nombreUsuario} registrado con exito");
                networkDataHelper.SendMessage(clientSocket, "El usuario fue registrado con exito.", CommandConstants.Registro);
            }
        }

        private static void ValidarLoginUsuario(NetworkDataHelper networkDataHelper, Socket clientSocket, string nombreLogin, string contraseña)
        {
            if(nombreLogin == "" || contraseña == "")
            {
                networkDataHelper.SendMessage(clientSocket, "Ningun campo puede ser vacio.", CommandConstants.Login);
                Console.WriteLine("Logueo incorrecto por campos vacios.");
            }
            else if (!_usuarios.Exists(u => u.PNomUsu == nombreLogin))
            {
                networkDataHelper.SendMessage(clientSocket, "No existe el usuario con el que se quiere loguear.", CommandConstants.Login);
                Console.WriteLine("Logueo incorrecto por usuario inexistente.");
            }
            else if(!_usuarios.Exists(u => u.Pass == contraseña && u.PNomUsu == nombreLogin))
            {
                networkDataHelper.SendMessage(clientSocket, "Contraseña incorrecta", CommandConstants.Login);
                Console.WriteLine("Logueo incorrecto por contraseña incorrecta");
            }
            else if(_usuarios.Exists(u => u.PNomUsu == nombreLogin && u.Pass == contraseña && u.Habilitado == false))
            {
                networkDataHelper.SendMessage(clientSocket, "El usuario se encuentra inhabilitado.", CommandConstants.Login);
                Console.WriteLine("Logueo denegado por usuario no habilitado");
            }
            else
            {
                networkDataHelper.SendMessage(clientSocket, "El usuario se logueo correctamente.", CommandConstants.Login);
                Console.WriteLine("El usuario se logueo correctamente.");
            }
        }

        private static bool ExistenUsuariosHabilitados()
        {
            return _usuarios.Any(us => us.Habilitado == true);
        }

        private static bool ExistenUsuariosNoHabilitados()
        {
            return _usuarios.Any(us => us.Habilitado == false);
        }

        private static void BusquedaUsuarios(Socket clientSocket, string caracteres, int constante)
        {
            caracteres = caracteres.ToLower();
            var totalUsuarios = "";
            if(_usuarios.Count == 0)
            {
                networkDataHelper.SendMessage(clientSocket, $"{totalUsuarios}", constante);
                return;
            }
            else if (caracteres == "" && constante == CommandConstants.BusquedaIncluyente)
            {
                networkDataHelper.SendMessage(clientSocket, $"{totalUsuarios}", constante);
                Console.WriteLine("Busqueda Finalizada");
                return;
            }
            else
            {
                for (int i = 0; i < _usuarios.Count; i++)
                {
                    var nombreDeUsuario = _usuarios[i].PNomUsu.ToLower();
                    var nombreDeUsuarioReal = _usuarios[i].PNomReal.ToLower();
                    if(constante == CommandConstants.BusquedaIncluyente)
                    {
                        if (nombreDeUsuario.Contains(caracteres) || nombreDeUsuarioReal.Contains(caracteres))
                        {
                            if (totalUsuarios == "")
                                totalUsuarios += $"{_usuarios[i]}";
                            else
                                totalUsuarios += $"?{_usuarios[i]}";
                        }
                    }
                    if(constante == CommandConstants.BusquedaExcluyente)
                    {
                        if(caracteres == "")
                        {
                            if (totalUsuarios == "")
                                totalUsuarios += $"{_usuarios[i]}";
                            else
                                totalUsuarios += $"?{_usuarios[i]}";
                        }
                        else
                        {
                            if (!nombreDeUsuario.Contains(caracteres) && !nombreDeUsuarioReal.Contains(caracteres))
                            {
                                if (totalUsuarios == "")
                                    totalUsuarios += $"{_usuarios[i]}";
                                else
                                    totalUsuarios += $"?{_usuarios[i]}";
                            }
                        }
                    }
                }
            }
            networkDataHelper.SendMessage(clientSocket, $"{totalUsuarios}", constante);
            Console.WriteLine("Busqueda Finalizada");
        }
    }
}
