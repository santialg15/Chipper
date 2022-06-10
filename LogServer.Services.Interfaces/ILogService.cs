using LogServer.Models;

namespace LogServer.Services.Interfaces
{
    public interface ILogService
    {
        List<Log> GetLogByUser(string user);
        List<Log> GetLogByChipKey(string key);
        List<Log> GetLogByDate(string date);
        List<Log> GetLogByAction(string action);
    }
}