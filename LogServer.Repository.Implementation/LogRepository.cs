using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using LogServer.Models;
using LogServer.Repository.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LogServer.Repository.Implementation
{
    public class LogRepository : ILogRepository
    {
        private List<Log> _logs = new List<Log>();
        private int logId = 0;
        private readonly object lockId = new object();

        public void AddLog(Log log)
        {
            lock (lockId)
            {
                logId++;
                log.id = logId;
                log.receive = DateTime.Now.ToString();
                _logs.Add(log);
            }
           
        }

        public List<Log> GetLogByUser(string user)
        {

            List<Log> ret = new List<Log>();
            foreach (var l in _logs)
            {
                if (l.user.Equals(user))
                {
                    ret.Add(l);
                }
            }

            return ret;
        }

        public List<Log> GetLogByChipKey(string key)
        {
            List<Log> ret = new List<Log>();
            foreach (var l in _logs)
            {
                if (l.message.Contains(key) && l.action.Equals("Nuevo chip"))
                {
                     ret.Add(l);
                }
            }
            return ret;
        }

        public List<Log> GetLogByDate(string date)
        {
          
            List <Log> ret = new List<Log>();
            foreach (var l in _logs)
            {
                var dateString = l.send.Remove(9);
                dateString = dateString.Replace("/", "");
                if (dateString.Contains(date.Trim()))
                {
                    ret.Add(l);
                }
            }
            return ret;
        }


        public List<Log> GetLogByAction(string action)
        {
            List<Log> ret = new List<Log>();
            foreach (var l in _logs)
            {
                if (l.action.Equals(action.Trim()))
                {
                    ret.Add(l);
                }
            }
            return ret;
        }

        public List<Log> GetAllLogs()
        {
            return _logs;
        }
    }
}