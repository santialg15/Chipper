using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using ProyectoCompartido.Logs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LogServer
{
    internal static class serverLog
    {
        static List<Log> _logs = new List<Log>();
        static void Main(string[] args)
        {

            using var channel = new ConnectionFactory() { HostName = "localhost" }.CreateConnection().CreateModel();
            channel.QueueDeclare(queue: "log_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var log = JsonSerializer.Deserialize<Log>(message);
                log.receive = DateTime.Now.ToString();
                _logs.Add(log);
                Console.WriteLine(" [x] Usuario: [{0}], Acción: [{1}], Mensaje: [{2}], Enviado: [{3}], recivido en servidor: [{4}] ",
                    log.user, log.action, log.message, log.send, log.receive);
            };
            channel.BasicConsume(queue: "log_queue",
                autoAck: true,
                consumer: consumer);


            printMenu();
            var exit = false;
            while (!exit){
            var op = Console.ReadLine();
            var filtro = "";
            switch (op)
            {
                case "1":
                    Console.WriteLine("Ingrese el usuario:");
                    filtro = Console.ReadLine();
                    foreach (var l in _logs)
                    {
                        if (l.user.Equals(filtro))
                        {
                            Console.WriteLine(l.ToString());
                        }
                    }
                    break;
                case "2":
                    Console.WriteLine("Ingrese palabra de chip:");
                    filtro = Console.ReadLine();
                    foreach (var l in _logs)
                    {
                        if (l.message.Equals(filtro) && l.action.Equals("Nuevo chip"))
                        {
                            Console.WriteLine(l.ToString());
                        }
                    }
                    break;
                case "3":
                    Console.WriteLine("Ingrese Fecha con formato dd/mm/yyyy:");
                    filtro = Console.ReadLine();
                    foreach (var l in _logs)
                    {
                        if (l.send.Contains(filtro.Trim()))
                        {
                            Console.WriteLine(l.ToString());
                        }
                    }
                    break;
                case "4":
                    Console.WriteLine("Ingrese acción:");
                    filtro = Console.ReadLine();
                    foreach (var l in _logs)
                    {
                        if (l.action.Equals(filtro.Trim()))
                        {
                            Console.WriteLine(l.ToString());
                        }
                    }
                    break;
                case "exit":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Opción invalida");
                    break;

            }
            printMenu();
            }
        }

        private static void printMenu()
        {
            Console.WriteLine("'exit' para salir");
            Console.WriteLine("Filtrar logs por:");
            Console.WriteLine("1 -> Usuario");
            Console.WriteLine("2 -> Palabra clave en chip");
            Console.WriteLine("3 -> Fecha");
            Console.WriteLine("4 -> Acción");
        }


    }
}