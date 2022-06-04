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
                    log.user, log.action, log.message,log.send,log.receive);
            };
            channel.BasicConsume(queue: "log_queue",
                autoAck: true,
                consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}