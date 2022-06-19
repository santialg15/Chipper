using LogServer.Models;

namespace LogServer.Repository.Interfaces
{
    public interface ILogRepository
    {
        List<Log> GetLogByUser(string user);
        List<Log> GetLogByChipKey(string key);
        List<Log> GetLogByDate(string date);
        List<Log> GetLogByAction(string action);
        void AddLog(Log log);
        List<Log> GetAllLogs();

    }
}