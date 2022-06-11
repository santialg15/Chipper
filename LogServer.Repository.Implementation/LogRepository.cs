using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using LogServer.Models;
using LogServer.Repository.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LogServer.Repository.Implementation
{
    public class LogRepository : ILogRepository
    {
        private List<Log> _logs = new List<Log>();
        private EventingBasicConsumer consumer = null;
        private IModel channel = null;


        public void ReceiveLog()
        {
            if (consumer == null){
                channel = new ConnectionFactory() { HostName = "localhost" }.CreateConnection().CreateModel();
                channel.QueueDeclare(queue: "log_queue",
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

                consumer = new EventingBasicConsumer(channel);
                
           

            consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var log = JsonSerializer.Deserialize<Log>(message);
                        log.receive = DateTime.Now.ToString();
                        _logs.Add(log);
                    };
            channel.BasicConsume(queue: "log_queue",
            autoAck: true,
            consumer: consumer);
            }
        }



        public List<Log> GetLogByUser(string user)
        {
            ReceiveLog();

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
            ReceiveLog();
            List<Log> ret = new List<Log>();
            foreach (var l in _logs)
            {
                if (l.message.Contains(key) && l.action.Equals("Nuevo chip"))
                {
                     ret.Add(l);
                }
            }
            return ret;
        }

        public List<Log> GetLogByDate(string date)
        {
            ReceiveLog();
          
            List <Log> ret = new List<Log>();
            foreach (var l in _logs)
            {
                var dateString = l.send.Remove(9);
                dateString = dateString.Replace("/", "");
                if (dateString.Contains(date.Trim()))
                {
                    ret.Add(l);
                }
            }
            return ret;
        }


        public List<Log> GetLogByAction(string action)
        {
            ReceiveLog();
            List<Log> ret = new List<Log>();
            foreach (var l in _logs)
            {
                if (l.action.Equals(action.Trim()))
                {
                    ret.Add(l);
                }
            }
            return ret;
        }
    }
}