using Protocolo;
using Protocolo.Interfaces;
using servidor;
using System.Net;
using System.Net.Sockets;


namespace Servidor
{
    internal static class Servidor
    {
        static readonly ISettingsManager SettingsMgr = new SettingsManager();
        static bool _exit = false;
        static List<TcpClient> _clients = new List<TcpClient>();
        private static List<Usuario> _usuarios = new List<Usuario>();
        private static NetworkDataHelper networkDataHelper;
        private static readonly object _lockUsuarios = new object();

        static void Main(string[] args)
        {
            agregarDatos();


            var ipEndPoint = new IPEndPoint(
                IPAddress.Parse(SettingsMgr.ReadSetting(ServerConfig.ServerIpConfigKey)),
                int.Parse(SettingsMgr.ReadSetting(ServerConfig.SeverPortConfigKey)));

            var tcpListener = new TcpListener(ipEndPoint);

            tcpListener.Start(100);

            //Lanzar un thread para manejar las conexiones
            Task escucharConexiones = Task.Run(() => ListenForConnections(tcpListener));
            ////var threadServer = new Thread(() => ListenForConnections(tcpListener));
            ////threadServer.Start();


            Console.WriteLine("Bienvenido al Sistema Server");
            printMenu();
            while (!_exit)
            {
                var userInput = Console.ReadLine();
                switch (userInput)
                {
                    case "1": // SRF1
                        _exit = true;
                        tcpListener.Stop();
                        foreach (var client in _clients)
                        {
                            client.Close();
                        }
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
                                    ret += usu4.getNomUsu() + ":3 " + pub.getContenido() + "\n";
                                }
                            }
                        }

                        if (ret.Equals(""))
                        {
                            Console.WriteLine("No se encontraron chips que contengan: " + buscar.Trim());
                        }

                        Console.WriteLine(ret);
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
                        if (_usuarios.Count == 0)
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
                        if (nombreANegar == null || nombreANegar == "")
                        {
                            Console.WriteLine("El usuario ingresado no existe.");
                            printMenu();
                            break;
                        }
                        var indiceNegar = _usuarios.FindIndex(u => u.PNomReal == nombreANegar);
                        if (indiceNegar == -1)
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
                            if (u.Habilitado == false)
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
                        IEnumerable<Usuario> colUsuarios = _usuarios.OrderByDescending(usuario => usuario.GetCantPubEnTmpConf());
                        int cont = 0;
                        foreach (var usu in colUsuarios)
                        {
                            Console.WriteLine(usu.ToString());
                            cont++;
                            if (cont == Int32.Parse(SettingsMgr.ReadSetting(ServerConfig.SeverTopMasUsosConfigKey)))
                            {
                                break;
                            }
                        }
                        printMenu();
                        break;
                    default:
                        Console.WriteLine("Opcion incorrecta ingresada");
                        break;
                }
            }
        }

        private static void agregarDatos()
        {
            //Usuarios
            Usuario _usu1 = new Usuario("Denis", "dpena", "inicio", "img");
            Usuario _usu2 = new Usuario("Santiago", "salvarez", "inicio", "img");
            Usuario _usu3 = new Usuario("Prueba", "pprueba", "inicio", "img");

            _usuarios.Add(_usu1);
            _usuarios.Add(_usu2);
            _usuarios.Add(_usu3);

            SeguirUnUsuarioParaTest("dpena", "salvarez");
            SeguirUnUsuarioParaTest("dpena", "pprueba");
            SeguirUnUsuarioParaTest("salvarez", "dpena");


            //Notificaciones
            Publicacion p1 = new Publicacion("Publicacion 1", _usu2.ColPublicacion.Count);
            Publicacion p2 = new Publicacion("Publicacion 2", _usu2.ColPublicacion.Count);
            Publicacion p3 = new Publicacion("Publicacion 3", _usu2.ColPublicacion.Count);
            Publicacion p4 = new Publicacion("Publicacion 4", _usu2.ColPublicacion.Count);
            Publicacion p5 = new Publicacion("Publicacion 5", _usu2.ColPublicacion.Count);
            Publicacion p6 = new Publicacion("Publicacion 6", _usu2.ColPublicacion.Count);

            _usu2.nuevoChip("Publicacion 1");
            _usu2.nuevoChip("Publicacion 2");
            _usu2.nuevoChip("Publicacion 3");
            _usu2.nuevoChip("Publicacion 4");
            _usu2.nuevoChip("Publicacion 5");
            _usu1.nuevoChip("Publicacion 6");

            _usu1.AddNotif(p1);
            _usu1.AddNotif(p2);
            _usu1.AddNotif(p3);
            _usu1.AddNotif(p4);
            _usu1.AddNotif(p5);
            _usu2.AddNotif(p6);

        }


        private static void printMenu()
        {
            Console.WriteLine("1 -> abandonar el programa");
            Console.WriteLine("2 -> listar usuarios");
            Console.WriteLine("3 -> Buscar chips");
            Console.WriteLine("4 -> top " + SettingsMgr.ReadSetting(ServerConfig.SeverTopSeguidoresConfigKey) + " con mas seguidores");
            Console.WriteLine("5 -> Negar acceso a usuario");
            Console.WriteLine("6 -> Permitir acceso a usuario");
            Console.WriteLine("7 -> top " + SettingsMgr.ReadSetting(ServerConfig.SeverTopMasUsosConfigKey) + " que mas usaron el sistema en los ultimos " + SettingsMgr.ReadSetting(ServerConfig.SeverTmpMostrarPubConfigKey) + " minutos");
            Console.WriteLine("Ingrese el numero de la opcion deseada: ");
        }


        private static void ListenForConnections(TcpListener tcpListener)
        {
            while (!_exit)
            {
                try
                {
                    var clientConnected = tcpListener.AcceptTcpClient();
                    _clients.Add(clientConnected);

                    networkDataHelper = new NetworkDataHelper(clientConnected);

                    Console.WriteLine("Nueva conexion aceptada...");
                    Task nuevoCliente = Task.Run(() => { HandleClient(clientConnected); });

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    _exit = true;
                }
            }
            Console.WriteLine("Exiting....");
        }


        private static void HandleClient(TcpClient tcpClient)
        {
            try
            {
                while (!_exit)
                {
                    //var headerLength = HeaderConstants.Request.Length + HeaderConstants.CommandLength +
                    //                   HeaderConstants.DataLength;
                    //var buffer = new byte[headerLength];
                    //try
                    //{
                    var (data, header) = networkDataHelper.ReceiveData();
                    //var header = new Header();
                    //_exit = !header.DecodeData(buffer);

                    switch (header.ICommand)
                    {
                        case CommandConstants.exit:
                            _exit = true;
                            Console.WriteLine("Terminó la conexión de un cliente");
                            break;
                        case CommandConstants.Registro:
                            Console.WriteLine("Validando registro de un usuario en el sistema");
                            //var datosRegistro = ObtenerDatosDelCliente(header, tcpClient);
                            var datosSeparados = data.Split("?");
                            var nombreUsuario = datosSeparados[0];
                            var nombreReal = datosSeparados[1];
                            var contraseña = datosSeparados[2];
                            ValidarRegistroUsuario(tcpClient, nombreUsuario, nombreReal, contraseña);
                            break;

                        case CommandConstants.Login:
                            Console.WriteLine("Validando ingreso de usuario en el sistema");
                            //var datosLogin = ObtenerDatosDelCliente(header, tcpClient);
                            var datosLoginSeparados = data.Split("?");
                            var nombreLogin = datosLoginSeparados[0];
                            var contraseñaLogin = datosLoginSeparados[1];
                            ValidarLoginUsuario(networkDataHelper, tcpClient, nombreLogin, contraseñaLogin);
                            break;

                        case CommandConstants.BusquedaIncluyente:
                            Console.WriteLine("El usuario inicio una busqueda de usuarios...");
                            var datosBusquedaIncluyente = data; //ObtenerDatosDelCliente(header, tcpClient);
                            BusquedaUsuarios(tcpClient, datosBusquedaIncluyente,
                                CommandConstants.BusquedaIncluyente);
                            break;

                        case CommandConstants.BusquedaExcluyente:
                            Console.WriteLine("El usuario inicio una busqueda de usuarios...");
                            var datosBusquedaExcluyente = data; //ObtenerDatosDelCliente(header, tcpClient);
                            BusquedaUsuarios(tcpClient, datosBusquedaExcluyente,
                                CommandConstants.BusquedaExcluyente);
                            break;

                        case CommandConstants.SeguirUsuario:
                            Console.WriteLine("El usuario quiere seguir a un usuario...");
                            //var datosSeguirUsuario = ObtenerDatosDelCliente(header, tcpClient);
                            var seguidorYaSeguir = data.Split("?");
                            SeguirUnUsuario(tcpClient, seguidorYaSeguir[0], seguidorYaSeguir[1]);
                            break;
                        case CommandConstants.chip:
                            //var dLogin = ObtenerDatosDelCliente(header, tcpClient);
                            var dSeparados = data.Split("?");
                            var nomUsu = dSeparados[0];
                            var CntImg = dSeparados[1];
                            var chip = dSeparados[2];
                            Publicacion nuevaPub;
                            Usuario usuChip = buscarUsuarioLogin(nomUsu);
                            if (int.Parse(CntImg) > 0)
                            {
                                //var serverHandler = new ServerHandler();
                                //serverHandler.StartClient();
                                int contadorImg = 1;
                                var colFileName = "";
                                while (contadorImg < int.Parse(CntImg))
                                {
                                    var fileName = networkDataHelper.ReceiveFile();
                                    colFileName += fileName + "?";
                                    contadorImg++;
                                }
                                //serverHandler.stop();
                                colFileName = colFileName.Remove(colFileName.Length - 1);
                                nuevaPub = usuChip.nuevoChipConImg(chip, colFileName);

                            }
                            else
                            {
                                nuevaPub = usuChip.nuevoChip(chip);
                            }

                            Notificar(usuChip, nuevaPub);

                            break;

                        case CommandConstants.VerChips:
                            Console.WriteLine("Procesando solicitud de visualizacion de chips...");
                            var nombreDeUsuario = data; //ObtenerDatosDelCliente(header, tcpClient);
                            VerChipsDeUsuario(tcpClient, nombreDeUsuario);
                            break;

                        case CommandConstants.ResponderChip:
                            Console.WriteLine("Procesando solicitud de respuesta de chip...");
                            var datosRespuestaChip = data; // ObtenerDatosDelCliente(header, tcpClient);
                            ResponderChip(tcpClient, datosRespuestaChip);
                            break;

                        case CommandConstants.verNotif:
                            Console.WriteLine("Procesando solicitud de notificaciones...");
                            nomUsu = data; //ObtenerDatosDelCliente(header, tcpClient);
                            var usu = _usuarios.Find(u => u.PNomUsu == nomUsu);
                            var totalNotif = "";

                            var notif = usu.getColNotif;
                            for (int i = 0; i < notif.Count; i++)
                            {
                                totalNotif += notif[i].ToString() + "?";
                            }

                            if (totalNotif.Equals(""))
                            {
                                networkDataHelper.SendMessage("No hay notificaciones",
                                    CommandConstants.verNotif);
                            }
                            else
                            {
                                networkDataHelper.SendMessage(totalNotif, CommandConstants.verNotif);
                            }

                            usu.clearNotif();

                            Console.WriteLine("Funcionalidad ver notificaciones finalizada.");
                            break;
                    }
                }
                //catch (Exception e)
                //{
                //    Console.WriteLine(
                //        $"Server: Server is closing, will not process more data -> Message {e.Message}..");
                //    _exit = true;
                //}
                //}
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Server: The client connection was interrupted - Exception {e.Message}");
                _exit = false;
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


        //private static string ObtenerDatosDelCliente(Header header, TcpClient tcpClient)
        //{
        //    var datosBuffer = new byte[header.IDataLength];
        //    networkDataHelper.ReceiveData(tcpClient);
        //    return Encoding.UTF8.GetString(datosBuffer);
        //}


        private static void ValidarRegistroUsuario(TcpClient tcpClient, string nombreUsuario, string nombreReal, string contraseña)
        {
            if (nombreUsuario == "" || nombreReal == "" || contraseña == "")
            {
                networkDataHelper.SendMessage("Ningun campo puede estar vacio.", CommandConstants.Registro);
                Console.WriteLine("No se realizo el registro de usuario.");
            }
            else
            {
                lock (_lockUsuarios)
                {
                    if (_usuarios.Exists(u => u.PNomUsu == nombreUsuario))
                    {
                        networkDataHelper.SendMessage("El usuario ya existe.", CommandConstants.Registro);
                        Console.WriteLine("No se realizo el registro de usuario.");
                    }
                    else
                    {
                        Usuario nuevoUsuario = new Usuario(nombreReal, nombreUsuario, contraseña, "imagen");
                        _usuarios.Add(nuevoUsuario);
                        Console.WriteLine($"Usuario {nombreUsuario} registrado con exito");
                        networkDataHelper.SendMessage("El usuario fue registrado con exito.", CommandConstants.Registro);
                    }
                }
            }
        }


        private static void ValidarLoginUsuario(NetworkDataHelper networkDataHelper, TcpClient tcpClient, string nombreLogin, string contraseña)
        {
            if (nombreLogin == "" || contraseña == "")
            {
                networkDataHelper.SendMessage("Ningun campo puede ser vacio.", CommandConstants.Login);
                Console.WriteLine("Logueo incorrecto por campos vacios.");
            }
            else if (!_usuarios.Exists(u => u.PNomUsu == nombreLogin))
            {
                networkDataHelper.SendMessage("No existe el usuario con el que se quiere loguear.", CommandConstants.Login);
                Console.WriteLine("Logueo incorrecto por usuario inexistente.");
            }
            else if (!_usuarios.Exists(u => u.Pass == contraseña && u.PNomUsu == nombreLogin))
            {
                networkDataHelper.SendMessage("Contraseña incorrecta", CommandConstants.Login);
                Console.WriteLine("Logueo incorrecto por contrasena incorrecta");
            }
            else if (_usuarios.Exists(u => u.PNomUsu == nombreLogin && u.Pass == contraseña && u.Habilitado == false))
            {
                networkDataHelper.SendMessage("El usuario se encuentra inhabilitado.", CommandConstants.Login);
                Console.WriteLine("Logueo denegado por usuario no habilitado");
            }
            else
            {
                networkDataHelper.SendMessage("El usuario se logueo correctamente.", CommandConstants.Login);
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


        private static void BusquedaUsuarios(TcpClient tcpClient, string caracteres, int constante)
        {
            caracteres = caracteres.ToLower();
            var totalUsuarios = "";
            if (_usuarios.Count == 0)
            {
                networkDataHelper.SendMessage($"{totalUsuarios}", constante);
                return;
            }
            else if (caracteres == "" && constante == CommandConstants.BusquedaIncluyente)
            {
                networkDataHelper.SendMessage($"{totalUsuarios}", constante);
                Console.WriteLine("Busqueda Finalizada");
                return;
            }
            else
            {
                for (int i = 0; i < _usuarios.Count; i++)
                {
                    var nombreDeUsuario = _usuarios[i].PNomUsu.ToLower();
                    var nombreDeUsuarioReal = _usuarios[i].PNomReal.ToLower();
                    if (constante == CommandConstants.BusquedaIncluyente)
                    {
                        if (nombreDeUsuario.Contains(caracteres) || nombreDeUsuarioReal.Contains(caracteres))
                        {
                            if (totalUsuarios == "")
                                totalUsuarios += $"{_usuarios[i]}";
                            else
                                totalUsuarios += $"?{_usuarios[i]}";
                        }
                    }
                    if (constante == CommandConstants.BusquedaExcluyente)
                    {
                        if (caracteres == "")
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
            networkDataHelper.SendMessage($"{totalUsuarios}", constante);
            Console.WriteLine("Busqueda Finalizada");
        }


        private static void Notificar(Usuario usuChip, Publicacion chip)
        {
            // Busco en usuarios seguidos del usuario que crea chip (usuChip)
            foreach (var usuSeguidos in usuChip.getColSeguidores)
            {
                // Actualizo lista de usuarios agregando norificación a los que siguen al usuario creador del chip
                foreach (var usu in _usuarios)
                {
                    if (!usu.Equals(usuSeguidos))
                    {
                        usu.AddNotif(chip);
                    }
                }
            }
        }

        private static void SeguirUnUsuario(TcpClient tcpClient, string nombreUsuario, string aSeguir)
        {
            var existeUsuario = _usuarios.Any(u => u.PNomUsu == nombreUsuario);
            var existeASeguir = _usuarios.Any(u => u.PNomUsu == aSeguir);
            if (!existeUsuario || !existeASeguir)
            {
                networkDataHelper.SendMessage("", CommandConstants.SeguirUsuario);
            }
            else
            {
                foreach (var usu in _usuarios)
                {
                    if (usu.PNomUsu == nombreUsuario)
                    {
                        var usuASeguir = usu.ColSeguidos.Find(u => u.PNomUsu == aSeguir);
                        if (usuASeguir == null)
                        {
                            var seguido = _usuarios.Find(u => u.PNomUsu == aSeguir);
                            usu.ColSeguidos.Add(seguido);
                            seguido.ColSeguidores.Add(usu);
                        }
                        else
                        {
                            networkDataHelper.SendMessage("Ya sigue a este usuario.", CommandConstants.SeguirUsuario);
                            break;
                        }
                        networkDataHelper.SendMessage("El usuario fue agregado.", CommandConstants.SeguirUsuario);
                        break;
                    }
                }
            }
            Console.WriteLine("Funcionalidad seguir usuario finalizada.");
        }


        private static void SeguirUnUsuarioParaTest(string nombreUsuario, string aSeguir)
        {
            foreach (var usu in _usuarios)
            {
                if (usu.PNomUsu == nombreUsuario)
                {
                    var usuASeguir = usu.ColSeguidos.Find(u => u.PNomUsu == aSeguir);
                    if (usuASeguir == null)
                    {
                        var seguido = _usuarios.Find(u => u.PNomUsu == aSeguir);
                        usu.ColSeguidos.Add(seguido);
                        seguido.ColSeguidores.Add(usu);
                    }
                }
            }
        }

        private static void VerChipsDeUsuario(TcpClient tcpClient, string nombreUsuario)
        {
            var usuarioElegido = _usuarios.Find(u => u.PNomUsu == nombreUsuario);
            if (usuarioElegido == null)
            {
                networkDataHelper.SendMessage("El usuario no existe", CommandConstants.VerChips);
                return;
            }
            var chips = usuarioElegido.ColPublicacion;
            var totalChips = "";
            if (chips.Count > 0)
            {
                totalChips += $"{usuarioElegido.PNomUsu}?";
                for (int i = 0; i < chips.Count; i++)
                {
                    if (i - 1 == chips.Count)
                        totalChips += chips[i].ToString();
                    else
                    {
                        totalChips += chips[i].ToString() + "?";
                        var respuestas = chips[i].colRespuesta;
                        for (int r = 0; r < respuestas.Count; r++)
                        {
                            totalChips += $"{respuestas[r]}?";
                        }
                    }
                }
            }
            networkDataHelper.SendMessage(totalChips, CommandConstants.VerChips);
            Console.WriteLine("Funcionalidad ver chips finalizada.");
        }

        private static void ResponderChip(TcpClient tcpClient, string datosRespuestaChip)
        {
            var datosRespuesta = datosRespuestaChip.Split("?");
            var usuarioLogueado = datosRespuesta[0];
            var usuarioDeChip = datosRespuesta[1];
            var respuesta = datosRespuesta[2];
            var numeroChip = int.Parse(datosRespuesta[3]);
            Respuesta nuevaRespuesta = new Respuesta(usuarioLogueado, respuesta);

            foreach (var usuario in _usuarios)
            {
                if (usuario.PNomUsu == usuarioDeChip)
                {
                    if (numeroChip <= usuario.ColPublicacion.Count)
                    {
                        var publicacion = usuario.ColPublicacion[numeroChip - 1];
                        publicacion.colRespuesta.Add(nuevaRespuesta);
                        break;
                    }
                    else
                    {
                        networkDataHelper.SendMessage("No existe el numero de chip seleccionado.", CommandConstants.ResponderChip);
                        return;
                    }
                }
            }
            networkDataHelper.SendMessage("Respuesta creada correctamente.", CommandConstants.ResponderChip);
            Console.WriteLine("Finalizo funcionalidad de responder chip.");
        }
    }
}
