using System.Net;
using System.Net.Sockets;
using System.Text;
using Protocolo;

namespace Servidor
{
    internal static class Servidor
    {
        static bool _exit = false;
        static List<Socket> _clients = new List<Socket>();
        private static List<Usuario> _usuarios = new List<Usuario>();


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
                    case "1":
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
                    case "2":
                        foreach (var usu3 in _usuarios)
                        {
                            Console.WriteLine(usu3.ToString());
                        }
                        printMenu();
                        break;
                    case "5":
                        if(_usuarios.Count == 0)
                        {
                            Console.WriteLine("No hay usuarios registrados en el sistema.");
                            //imprimir menu
                            break;
                        }
                        if (!ExistenUsuariosHabilitados())
                        {
                            Console.WriteLine("No hay usuarios con premiso de acceso al sistema.");
                            //imprimir menu
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
                            //imprimir menu
                            break;
                        }
                        var indiceNegar = _usuarios.FindIndex(u => u.PNomUsu == nombreANegar);
                        if(indiceNegar == -1)
                        {
                            //imprimir menu
                            break;
                        }
                        _usuarios[indiceNegar].Habilitado = false;
                        Console.WriteLine($"Se le nego el acceso al usuario {nombreANegar}.");
                        //imprimir menu
                        break;
                    case "6":
                        if (_usuarios.Count == 0)
                        {
                            Console.WriteLine("No hay usuarios registrados en el sistema.");
                            break;
                        }
                        if (!ExistenUsuariosNoHabilitados())
                        {
                            Console.WriteLine("No hay usuarios con acceso denegado al sistema.");
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
                            //imprimir menu
                            break;
                        }
                        var indicePermitir = _usuarios.FindIndex(u => u.PNomUsu == nombreAPermitir);
                        if (indicePermitir == -1)
                        {
                            Console.WriteLine("El usuario ingresado no existe.");
                            //imprimir menu
                            break;
                        }
                        _usuarios[indicePermitir].Habilitado = true;
                        Console.WriteLine($"Se le permitio el acceso al usuario {nombreAPermitir}.");
                        //imprimir menu
                        break;
                    case "3":
                        Console.WriteLine("Ingrese su búsqueda: ");
                        var buscar = Console.ReadLine();
                        string ret = "";
                        foreach (var usu4 in _usuarios)
                        {
                            foreach (var pub in usu4.GetPublicaciones())
                            {
                                if (pub.getContenido().Contains(buscar.Trim()))
                                {
                                    ret += usu4.getNomUsu() + pub.getContenido() +"\n";
                                }
                            }
                        }

                        if (ret.Equals(""))
                        {
                            Console.WriteLine("No se encontraron chips que contengan: "+buscar.Trim());
                        }
                        printMenu();
                        break;
                        case "4":
                            _usuarios.OrderBy(cantSeg => cantSeg.GetCantSeg());
                            int contador = 0;
                            while(contador < 5 && contador < _usuarios.Count)
                            {
                                Console.WriteLine(_usuarios[contador].ToString());
                                contador++;
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
            Console.WriteLine("4 -> top 5 con más seguidores");
            Console.WriteLine("5 - Negar acceso a usuario");
            Console.WriteLine("6 - Permitir acceso a usuario");
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
                    Console.WriteLine("Accepted new connection...");
                    var threadcClient = new Thread(() => HandleClient(clientConnected));
                    threadcClient.Start();
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
                    ReceiveData(clientSocket, headerLength, buffer);
                    var header = new Header();
                    header.DecodeData(buffer);
                    switch (header.ICommand)
                    {
                        case CommandConstants.Registro:
                            Console.WriteLine("Validando registro de un usuario en el sistema");
                            var datosRegistro = ObtenerDatosDelCliente(header,clientSocket);
                            
                            var datosSeparados = datosRegistro.Split("?");
                            var nombreUsuario = datosSeparados[0];
                            var nombreReal = datosSeparados[1];
                            var contraseña = datosSeparados[2];
                            Usuario nuevoUsuario = new Usuario(nombreReal, nombreUsuario, contraseña, "imagen");
                            _usuarios.Add(nuevoUsuario);
                            Console.WriteLine($"Usuario {nombreUsuario} registrado con éxito");
                            break;
                        case CommandConstants.Login:
                            Console.WriteLine("Validando ingreso de usuario en el sistema");
                            var datosLogin = ObtenerDatosDelCliente(header, clientSocket);
                            var datosLoginSeparados = datosLogin.Split("?");
                            var nombreLogin = datosLoginSeparados[0];
                            var contraseñaLogin = datosLoginSeparados[1];
                            ValidarLoginUsuario(nombreLogin, contraseñaLogin);

                            Console.WriteLine("El usuario se logueo correctamente al sistema.");
                            //Enviar al cliente lista de posibles acciones luego de loguearse
                            break;
                        case CommandConstants.ListUsers:
                            for (int i = 0; i < _usuarios.Count; i++)
                            {
                                Console.WriteLine(_usuarios[i].ToString());

                            }
                            break;
                        case CommandConstants.Message:
                            Console.WriteLine("Will receive message to display...");
                            var bufferData = new byte[header.IDataLength];  
                            ReceiveData(clientSocket,header.IDataLength,bufferData);
                            Console.WriteLine("Message received: " + Encoding.UTF8.GetString(bufferData));
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Server is closing, will not process more data -> Message {e.Message}..");    
                }
            }
        }
        private static void ReceiveData(Socket clientSocket,  int Length, byte[] buffer)
        {
            var iRecv = 0;
            while (iRecv < Length)
            {
                try
                {
                    var localRecv = clientSocket.Receive(buffer, iRecv, Length - iRecv, SocketFlags.None);
                    if (localRecv == 0) // Si recieve retorna 0 -> la conexion se cerro desde el endpoint remoto
                    {
                        if (!_exit)
                        {
                            clientSocket.Shutdown(SocketShutdown.Both);
                            clientSocket.Close();
                        }
                        else
                        {
                            throw new Exception("Server is closing");
                        }
                    }

                    iRecv += localRecv;
                }
                catch (SocketException se)
                {
                    Console.WriteLine(se.Message);
                    return;
                }
            }
        }

        private static string ObtenerDatosDelCliente(Header header, Socket clientSocket)
        {
            var datosRegistroBuffer = new byte[header.IDataLength];
            ReceiveData(clientSocket, header.IDataLength, datosRegistroBuffer);
            return Encoding.UTF8.GetString(datosRegistroBuffer);
        }

        private static void ValidarLoginUsuario(string nombreLogin, string contraseña)
        {
            if(!_usuarios.Exists(u => u.PNomUsu == nombreLogin))
            {
                //ENVIAR MENSAJE AL CLIENTE QUE NO EXISTE EL USUARIO Y PERMITIRLE REPETIR PROCESO
                Console.WriteLine("no existe usuario");
            }
            else if(!_usuarios.Exists(u => u.Pass == contraseña && u.PNomUsu == nombreLogin))
            {
                //ENVIAR MENSAJE AL CLIENTE QUE NO COINCIDE LA CONTRASEÑA Y PERMITIRLE REPETIR PROCESO
                Console.WriteLine("contraseña incorrecta");
            }
            else
            {
                Usuario usuario = _usuarios.Find(u => u.PNomUsu == nombreLogin && u.Pass == contraseña);
                if(!usuario.Habilitado)
                {
                    //ENVIAR MENSAJE AL CLIENTE QUE EL USUARIO NO ESTA HABILITADO A LOGUERSE
                    Console.WriteLine("usuario no habilitado");
                }
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
    }
}
