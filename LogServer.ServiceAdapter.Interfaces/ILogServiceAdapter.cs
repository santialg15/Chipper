using LogServer.Models;
using LogServer.Responses;

namespace LogServer.ServiceAdapter.Interfaces
{
    public interface ILogServiceAdapter
    {
        List<LogResponse> GetLogByParameters(Parametros parametros);
    }
}