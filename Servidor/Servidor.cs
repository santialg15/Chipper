using System.Net;
using System.Net.Sockets;
using System.Text;
using Logica;
using ProyectoCompartido.Interfaces;
using ProyectoCompartido.Protocolo;
using ProyectoCompartido.Protocolo.FileTransfer;
using LogSender;
using LogServer.Models;

namespace Servidor
{
    internal static class Servidor
    {
        static readonly ISettingsManager SettingsMgr = new SettingsManager();
        static bool _exit = false;
        static List<TcpClient> _clients = new List<TcpClient>();
        private static List<Usuario> _usuarios = new List<Usuario>();
        private static readonly object _lockUsuarios = new object();
        private static readonly AsyncSenderQueue logSender = new AsyncSenderQueue();

        static async Task Main(string[] args)
        {
            agregarDatos();
            var serverIpAdress = IPAddress.Parse(SettingsMgr.ReadSetting(ServerConfig.ServerIpConfigKey));
            var serverPort = Int32.Parse(SettingsMgr.ReadSetting(ServerConfig.SeverPortConfigKey));
            var ipServerEndpoint = new IPEndPoint(serverIpAdress, serverPort);

            var tcpListener = new TcpListener(ipServerEndpoint);
            tcpListener.Start(100); //Acepta 100 clientes encolados sin tener que ser procesados.

            Task escucharConexiones = Task.Run(async () => await ListenForConnections(tcpListener));

            Console.WriteLine("Bienvenido al Sistema Server");
            printMenu();
            while (!_exit)
            {
                var userInput = Console.ReadLine();
                switch (userInput)
                {
                    case "1": // SRF1
                        _exit = true;
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
                        var indiceNegar = _usuarios.FindIndex(u => u.PNomUsu == nombreANegar);
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
                        var indicePermitir = _usuarios.FindIndex(u => u.PNomUsu == nombreAPermitir);
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

        private static async Task ListenForConnections(TcpListener tcpListener)
        {
            while (!_exit)
            {
                try
                {
                    var tcpClientSocket = await tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);
                    _clients.Add(tcpClientSocket);

                    Console.WriteLine("Nueva conexion aceptada...");
                    var task = Task.Run(async () => await HandleClient(tcpClientSocket).ConfigureAwait(false));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    _exit = true;
                }
            }
            Console.WriteLine("Exiting....");
        }

        private static void printMenu()
        {
            Console.WriteLine("1 -> abandonar el programa");
            Console.WriteLine("2 -> listar usuarios");
            Console.WriteLine("3 -> Buscar chips");
            Console.WriteLine("4 -> top " + SettingsMgr.ReadSetting(ServerConfig.SeverTopSeguidoresConfigKey) + " con mas seguidores");
            Console.WriteLine("5 -> Negar acceso a usuario");
            Console.WriteLine("6 -> Permitir acceso a usuario");
            Console.WriteLine("7 -> top " + SettingsMgr.ReadSetting(ServerConfig.SeverTopMasUsosConfigKey) + " que mas usaron el sistema en los ultimos minutos");
            Console.WriteLine("Ingrese el numero de la opcion deseada: ");
        }

        private static async void setLog(string user, string action, string msg)
        {
            Log log = new Log(user, action, msg);
            await logSender.sendLog(log);
        }

        private static async Task HandleClient(TcpClient clientSocket)
        {
            var clienteConectado = true;
            var usuLogueado = "";
            try
            {
                await using (var networkStream = clientSocket.GetStream())
                {
                    NetworkDataHelper networkDataHelper = new NetworkDataHelper(networkStream);
                    while (clienteConectado)
                    {
                        var headerLength = HeaderConstants.Request.Length + HeaderConstants.CommandLength +
                                           HeaderConstants.DataLength;
                        var buffer = new byte[headerLength];
                        await networkDataHelper.ReceiveData(buffer);
                        var fileheader = new FileHeader();
                        var header = new Header();
                        clienteConectado = header.DecodeData(buffer);

                        var datosBuffer = new byte[header.IDataLength];
                        await networkDataHelper.ReceiveData(datosBuffer);
                        var data = Encoding.UTF8.GetString(datosBuffer);

                        switch (header.ICommand)
                        {
                            case CommandConstants.exit:
                                clienteConectado = false;
                                Console.WriteLine("Terminó la conexión de un cliente");
                                setLog(usuLogueado, "exit", "El usuario salió del sistema");
                                break;
                            case CommandConstants.Registro:
                                Console.WriteLine("Validando registro de un usuario en el sistema");
                                var datosSeparados = data.Split("?");
                                var nombreUsuario = datosSeparados[0];
                                var nombreReal = datosSeparados[1];
                                var contraseña = datosSeparados[2];
                                await ValidarRegistroUsuario(networkDataHelper,nombreUsuario, nombreReal, contraseña);
                                break;

                            case CommandConstants.Login:
                                Console.WriteLine("Validando ingreso de usuario en el sistema");
                                var datosLogin = data;
                                var datosLoginSeparados = datosLogin.Split("?");
                                var nombreLogin = datosLoginSeparados[0];
                                var contraseñaLogin = datosLoginSeparados[1];
                                await ValidarLoginUsuario(networkDataHelper, nombreLogin, contraseñaLogin);
                                usuLogueado = buscarUsuarioLogin(nombreLogin).PNomUsu;
                                setLog(usuLogueado, "Login", "El usuario se logueo en el sistema");
                                break;

                            case CommandConstants.BusquedaIncluyente:
                                Console.WriteLine("El usuario inicio una busqueda de usuarios...");
                                var datosBusquedaIncluyente = data;
                                await BusquedaUsuarios(networkDataHelper, datosBusquedaIncluyente, CommandConstants.BusquedaIncluyente);
                                setLog(usuLogueado, "Busqueda incluye", datosBusquedaIncluyente);
                                break;

                            case CommandConstants.BusquedaExcluyente:
                                Console.WriteLine("El usuario inicio una busqueda de usuarios...");
                                var datosBusquedaExcluyente = data;
                                await BusquedaUsuarios(networkDataHelper, datosBusquedaExcluyente, CommandConstants.BusquedaExcluyente);
                                setLog(usuLogueado, "Busqueda excluye", data);
                                break;

                            case CommandConstants.SeguirUsuario:
                                Console.WriteLine("El usuario quiere seguir a un usuario...");
                                var datosSeguirUsuario = data;
                                var seguidorYaSeguir = datosSeguirUsuario.Split("?");
                                await SeguirUnUsuario(networkDataHelper, seguidorYaSeguir[0], seguidorYaSeguir[1]);
                                break;
                            case CommandConstants.chip:
                                var dSeparados = data.Split("?");
                                var nomUsu = dSeparados[0];
                                var CntImg = dSeparados[1];
                                var chip = dSeparados[2];
                                Publicacion nuevaPub;
                                Usuario usuChip = buscarUsuarioLogin(nomUsu);
                                if (int.Parse(CntImg) > 0)
                                {
                                    int contadorImg = 1;
                                    var colFileName = "";
                                    while (contadorImg < int.Parse(CntImg))
                                    {
                                        var fileName = networkDataHelper.ReceiveFile().Result;
                                        colFileName += fileName + "?";
                                        contadorImg++;
                                    }
                                    colFileName = colFileName.Remove(colFileName.Length - 1);
                                    nuevaPub = usuChip.nuevoChipConImg(chip, colFileName);
                                    setLog(usuLogueado, "Nuevo chip", chip+" imagenes: "+ CntImg.ToString());
                                }
                                else
                                {
                                    nuevaPub = usuChip.nuevoChip(chip);
                                    setLog(usuLogueado, "Nuevo chip", chip);
                                }
                                

                                Notificar(usuChip, nuevaPub);
                                break;

                            case CommandConstants.VerChips:
                                Console.WriteLine("Procesando solicitud de visualizacion de chips...");
                                var nombreDeUsuario = data;
                                await VerChipsDeUsuario(networkDataHelper, nombreDeUsuario);
                                setLog(usuLogueado, "ver chips", "Chips de usuario: "+ nombreDeUsuario);
                                break;

                            case CommandConstants.ResponderChip:
                                Console.WriteLine("Procesando solicitud de respuesta de chip...");
                                var datosRespuestaChip = data;
                                await ResponderChip(networkDataHelper, datosRespuestaChip);
                                break;

                            case CommandConstants.verNotif:
                                Console.WriteLine("Procesando solicitud de notificaciones...");
                                nomUsu = data;
                                var usu = _usuarios.Find(u => u.PNomUsu == nomUsu);
                                var totalNotif = "";

                                var notif = usu.getColNotif;
                                for (int i = 0; i < notif.Count; i++)
                                {
                                    totalNotif += notif[i].ToString() + "?";
                                }

                                if (totalNotif.Equals(""))
                                {
                                    await networkDataHelper.SendMessage("No hay notificaciones", CommandConstants.verNotif);
                                }
                                else
                                {
                                    await networkDataHelper.SendMessage(totalNotif, CommandConstants.verNotif);
                                }
                                setLog(usuLogueado, "ver Notificaciones", "Notificaciones: " + totalNotif.ToString());
                                usu.clearNotif();
                                Console.WriteLine("Funcionalidad ver notificaciones finalizada.");
                                break;
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine($"The client connection was interrupted - Exception {e.Message}");
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

        private static async Task ValidarRegistroUsuario(NetworkDataHelper networkDataHelper, string nombreUsuario, string nombreReal, string contraseña)
        {
            if (nombreUsuario == "" || nombreReal == "" || contraseña == "")
            {
                await networkDataHelper.SendMessage("Ningun campo puede estar vacio.", CommandConstants.Registro);
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
                        var fileName = networkDataHelper.ReceiveFile().Result;
                        Usuario nuevoUsuario = new Usuario(nombreReal, nombreUsuario, contraseña, fileName);
                        _usuarios.Add(nuevoUsuario);
                        Console.WriteLine($"Usuario {nombreUsuario} registrado con exito");
                        networkDataHelper.SendMessage("El usuario fue registrado con exito.", CommandConstants.Registro);
                        setLog(nombreUsuario, "registo de usuario", "El usuario se registró en el sistema");
                    }
                }
            }
        }

        private static async Task ValidarLoginUsuario(NetworkDataHelper networkDataHelper, string nombreLogin, string contraseña)
        {
            var usuarioLogueado = _usuarios.FirstOrDefault(u => u.PNomUsu == nombreLogin);
            if (nombreLogin == "" || contraseña == "")
            {
                await networkDataHelper.SendMessage("Ningun campo puede ser vacio.", CommandConstants.Login);
                Console.WriteLine("Logueo incorrecto por campos vacios.");
            }
            else if (!_usuarios.Exists(u => u.PNomUsu == nombreLogin))
            {
                await networkDataHelper.SendMessage("No existe el usuario con el que se quiere loguear.", CommandConstants.Login);
                Console.WriteLine("Logueo incorrecto por usuario inexistente.");
            }
            else if (!_usuarios.Exists(u => u.Pass == contraseña && u.PNomUsu == nombreLogin))
            {
                await networkDataHelper.SendMessage("Contraseña incorrecta", CommandConstants.Login);
                Console.WriteLine("Logueo incorrecto por contrasena incorrecta");
            }
            else if (_usuarios.Exists(u => u.PNomUsu == nombreLogin && u.Pass == contraseña && u.Habilitado == false))
            {
                await networkDataHelper.SendMessage("El usuario se encuentra inhabilitado.", CommandConstants.Login);
                Console.WriteLine("Logueo denegado por usuario no habilitado");
            }
            else if(usuarioLogueado != null && usuarioLogueado.estaLogueado == true)
            {
                await networkDataHelper.SendMessage("El usuario ya esta logueado.", CommandConstants.Login);
                Console.WriteLine("Un usuario ya logueado intento loguearse nuevamente.");
            }
            else
            {
                var usuario = _usuarios.FirstOrDefault(u => u.PNomUsu == nombreLogin);
                usuario.estaLogueado = true;
                await networkDataHelper.SendMessage("El usuario se logueo correctamente.", CommandConstants.Login);
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

        private static async Task BusquedaUsuarios(NetworkDataHelper networkDataHelper, string caracteres, int constante)
        {
            caracteres = caracteres.ToLower();
            var totalUsuarios = "";
            if (_usuarios.Count == 0)
            {
                await networkDataHelper.SendMessage($"{totalUsuarios}", constante);
                return;
            }
            else if (caracteres == "" && constante == CommandConstants.BusquedaIncluyente)
            {
                await networkDataHelper.SendMessage($"{totalUsuarios}", constante);
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
            await networkDataHelper.SendMessage($"{totalUsuarios}", constante);
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
                    if (usu.Equals(usuSeguidos))
                    {
                        usu.AddNotif(chip);
                    }
                }
            }
        }

        private static async Task SeguirUnUsuario(NetworkDataHelper networkDataHelper, string nombreUsuario, string aSeguir)
        {
            var existeUsuario = _usuarios.Any(u => u.PNomUsu == nombreUsuario);
            var existeASeguir = _usuarios.Any(u => u.PNomUsu == aSeguir);
            if (!existeUsuario || !existeASeguir)
            {
                await networkDataHelper.SendMessage("", CommandConstants.SeguirUsuario);
            }
            else if(nombreUsuario == aSeguir)
            {
                await networkDataHelper.SendMessage("No puedes seguirte a ti mismo.", CommandConstants.SeguirUsuario);
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
                            setLog(nombreUsuario, "Sigue a ", aSeguir);
                        }
                        else
                        {
                            await networkDataHelper.SendMessage("Ya sigue a este usuario.", CommandConstants.SeguirUsuario);
                            break;
                        }
                        await networkDataHelper.SendMessage("El usuario fue agregado.", CommandConstants.SeguirUsuario);
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

        private static async Task VerChipsDeUsuario(NetworkDataHelper networkDataHelper, string nombreUsuario)
        {
            var usuarioElegido = _usuarios.Find(u => u.PNomUsu == nombreUsuario);
            if (usuarioElegido == null)
            {
                await networkDataHelper.SendMessage("El usuario no existe", CommandConstants.VerChips);
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
            await networkDataHelper.SendMessage(totalChips, CommandConstants.VerChips);
            Console.WriteLine("Funcionalidad ver chips finalizada.");
        }

        private static async Task ResponderChip(NetworkDataHelper networkDataHelper, string datosRespuestaChip)
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
                        setLog(usuarioLogueado, "Responde chip", "Chip "+ numeroChip + " de usuario: " + usuarioDeChip);
                        break;
                    }
                    else
                    {
                        await networkDataHelper.SendMessage("No existe el numero de chip seleccionado.", CommandConstants.ResponderChip);
                        return;
                    }
                }
            }
            await networkDataHelper.SendMessage("Respuesta creada correctamente.", CommandConstants.ResponderChip);
            Console.WriteLine("Finalizo funcionalidad de responder chip.");
        }
    } 
}

