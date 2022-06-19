using LogServer.Models;
using LogServer.Responses;

namespace LogServer.ServiceAdapter.Interfaces
{
    public interface ILogServiceAdapter
    {
        List<LogResponse> GetLogByUser(string user);
        List<LogResponse> GetLogByChipKey(string key);
        List<LogResponse> GetLogByDate(string date);
        List<LogResponse> GetLogByAction(string action);
        List<LogResponse> GetLogByParameters(Parametros parametros);
    }
}