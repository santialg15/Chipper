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

        public List<LogResponse>  GetLogByUser(string user)
        {
            List<Log> Logs = _LogService.GetLogByUser(user);
            List<LogResponse> response = new List<LogResponse>();
            foreach (Log Log in Logs)
            {
                response.Add(MapModelToResponse(Log));
            }

            return response;
        }

        public List<LogResponse> GetLogByChipKey(string key)
        {
            List<Log> Logs = _LogService.GetLogByChipKey(key);
            List<LogResponse> response = new List<LogResponse>();
            foreach (Log Log in Logs)
            {
                response.Add(MapModelToResponse(Log));
            }

            return response;
        }

        public List<LogResponse> GetLogByDate(string date)
        {
            List<Log> Logs = _LogService.GetLogByDate(date);
            List<LogResponse> response = new List<LogResponse>();
            foreach (Log Log in Logs)
            {
                response.Add(MapModelToResponse(Log));
            }

            return response;
        }

        public List<LogResponse> GetLogByAction(string action)
        {
            List<Log> Logs = _LogService.GetLogByAction(action);
            List<LogResponse> response = new List<LogResponse>();
            foreach (Log Log in Logs)
            {
                response.Add(MapModelToResponse(Log));
            }

            return response;
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

            //BORRAR CREO QUE NO HAY REQUEST YA QUE NO ES UN CRUD, SOLO READ
        //private Log MapRequestToModel(LogRequest logRequest)
        //{
        //    return new Log()
        //    {
        //        user =
        //        action = 
        //        message = 
        //        send =
        //        receive = 

        //        Age = logRequest.Age,
        //        Creators = logRequest.Creators,
        //        Name = logRequest.Name,
        //        RealName = logRequest.RealName
        //    };
        //}
    }
}