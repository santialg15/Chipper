using System.Text;
using System.Text.Json;
using LogServer.Models;
using LogServer.Repository.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LogServer.Repository.Implementation
{
    public class LogRepository : ILogRepository
    {
        private List<Log> _logs = new List<Log>();

        public void reveiveLog()
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
                        //Console.WriteLine(" [x] Usuario: [{0}], Acción: [{1}], Mensaje: [{2}], Enviado: [{3}], recivido en servidor: [{4}] ",
                        //log.user, log.action, log.message, log.send, log.receive);
                    };
            channel.BasicConsume(queue: "log_queue",
            autoAck: true,
            consumer: consumer);
        }



        public List<Log> GetLogByUser(string user)
        {
            reveiveLog();

            List<Log> ret = new List<Log>();
            foreach (var l in _logs)
            {
                if (l.user.Equals(user))
                {
                    ret.Add(l);
                }
            }

            return ret;
        }

        public List<Log> GetLogByChipKey(string key)
        {
            reveiveLog();
            List<Log> ret = new List<Log>();
            foreach (var l in _logs)
            {
                if (l.message.Equals(key) && l.action.Equals("Nuevo chip"))
                {
                     ret.Add(l);
                }
            }
            return ret;
        }

        public List<Log> GetLogByDate(string date)
        {
            reveiveLog();
            List<Log> ret = new List<Log>();
            foreach (var l in _logs)
            {
                if (l.send.Contains(date.Trim()))
                {
                    Console.WriteLine(l.ToString());
                }
            }
            return ret;
        }

        public List<Log> GetLogByAction(string action)
        {
            reveiveLog();
            List<Log> ret = new List<Log>();
            foreach (var l in _logs)
            {
                if (l.action.Equals(action.Trim()))
                {
                    Console.WriteLine(l.ToString());
                }
            }
            return ret;
        }
    }
}