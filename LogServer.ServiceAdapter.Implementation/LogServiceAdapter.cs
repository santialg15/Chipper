using LogServer.Models;
using LogServer.Responses;
using LogServer.ServiceAdapter.Interfaces;
using LogServer.Services.Interfaces;

namespace LogServer.ServiceAdapter.Implementation
{
    public class LogServiceAdapter: ILogServiceAdapter
    {

        private readonly ILogService _LogService;

        public LogServiceAdapter(ILogService LogService)
        {
            _LogService = LogService;
        }

        private LogResponse MapModelToResponse(Log log)
        {
            return new LogResponse()
            {
                user = log.user,
                action = log.action,
                message = log.message,
                send = log.send,
                receive = log.receive,
                id = log.id
            };
        }

        public List<LogResponse> GetLogByParameters(Parametros parametros)
        {
            var nombreUsuario = parametros.usuario;
            var fecha = parametros.fecha;
            var accion = parametros.accion;
            var palabra = parametros.palabraChip;
            List<Log> logs = new List<Log>();
            List<LogResponse> logsARetornar = new List<LogResponse>();
            if(nombreUsuario == null && fecha == null && accion == null && palabra == null)
            {
                foreach (Log Log in _LogService.GetAllLogs())
                {
                    logsARetornar.Add(MapModelToResponse(Log));
                }
                return logsARetornar;
            }
            if (nombreUsuario != null)
            {
                List<Log> logsDeUsuario = _LogService.GetLogByUser(nombreUsuario);
                logs.AddRange(logsDeUsuario);
            }
            if(fecha != null)
            {
                List<Log> logsDeFecha = _LogService.GetLogByDate(fecha);
                logs.AddRange(logsDeFecha);
            }
            if(accion != null)
            {
                List<Log> logsDeAccion = _LogService.GetLogByAction(accion);
                logs.AddRange(logsDeAccion);
            }
            if(palabra != null)
            {
                List<Log> logsDePalabra = _LogService.GetLogByAction(palabra);
                logs.AddRange(logsDePalabra);
            }

            foreach (Log Log in logs.Distinct())
            {
                logsARetornar.Add(MapModelToResponse(Log));
            }
            return logsARetornar;
        }

    }
}