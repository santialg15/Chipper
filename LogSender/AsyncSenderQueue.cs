using System.Text;
using System.Text.Json;
using LogServer.Models;
using RabbitMQ.Client;

namespace LogSender
{
    public class AsyncSenderQueue
    {
        private IModel channel = null;

        public async Task sendLog(Log log)
        {
            if (channel == null){
                channel = new ConnectionFactory() { HostName = "localhost" }.CreateConnection().CreateModel();
            
                channel.QueueDeclare(queue: "log_queue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
            }

            var stringLog = JsonSerializer.Serialize(log);
            var result = await sendMessage(channel, stringLog);
            Console.WriteLine(result ? "Message {0} sent successfully" : "Could not send {0}", stringLog);

        }

        private static Task<bool> sendMessage(IModel channel, string message)
        {
            bool returnVal;
            try
            {
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "",
                    routingKey: "log_queue",
                    basicProperties: null,
                    body: body);
                returnVal = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                returnVal = false;
            }

            return Task.FromResult(returnVal);
        }
    }
}
