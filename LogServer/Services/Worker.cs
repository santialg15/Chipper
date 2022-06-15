using LogServer.Models;
using LogServer.Services.RabbitMQService;
using LogServer.Repository.Implementation;
using LogServer.Repository.Interfaces;

namespace LogServer.Services
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IBus _busControl;
        private readonly IServiceProvider _serviceProvider;
        
        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider){
            _serviceProvider = serviceProvider;
            _logger = logger;
            _busControl = RabbitHutch.CreateBus("amqps://arefvfdf:ez6JGlDGbYumDMT2TFiifmmNRVkHSA_o@beaver.rmq.cloudamqp.com/arefvfdf");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _busControl.ReceiveAsync<Log>(Queue.ProcessingQueueName, x =>
            {
                Task.Run(() => { ReceiveItem(x); }, stoppingToken);
            });
        }

        private void ReceiveItem(Log log)
        {
           try
           {
               ILogRepository logRepository = _serviceProvider.GetService<ILogRepository>();
               logRepository.AddLog(log);
           }
           catch (Exception e)
           {
               _logger.LogInformation($"Exception {e.Message} -> {e.StackTrace}");
           }
        }
    }
}