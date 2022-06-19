using LogServer.Models;
using LogServer.Repository.Interfaces;
using LogServer.Services.Interfaces;

namespace LogServer.Services.Implementation
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _LogRepository;

        public LogService(ILogRepository logRepository)
        {
            _LogRepository = logRepository;
        }

        public List<Log> GetLogByUser(string user)
        {
            return _LogRepository.GetLogByUser(user);
        }

        public List<Log> GetLogByChipKey(string key)
        {
            return _LogRepository.GetLogByChipKey(key);
        }

        public List<Log> GetLogByDate(string date)
        {
            return _LogRepository.GetLogByDate(date);
        }

        public List<Log> GetLogByAction(string action)
        {
            return _LogRepository.GetLogByAction(action);
        }

        public List<Log> GetAllLogs()
        {
            return _LogRepository.GetAllLogs();
        }
    }
}