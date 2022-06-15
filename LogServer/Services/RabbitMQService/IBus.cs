using System;
using System.Threading.Tasks;

namespace LogServer.Services.RabbitMQService
{
    public interface IBus
    {
        Task ReceiveAsync<T>(string queue, Action<T> onMessage);
    }
}