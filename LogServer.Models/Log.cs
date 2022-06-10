namespace LogServer.Models
{
    public class Log
    {
        public int id { get; set; }
        public string user { get; set; }
        public string action { get; set; }
        public string message { get; set; }
        public string send { get; set; }
        public string receive { get; set; }

        public Log(string _user, string _action, string _message)
        {
            user = _user;
            action = _action;
            message = _message;
            send = DateTime.Now.ToString();
            receive = "";
        }

        public Log()
        {
        }

        public override string ToString()
        {
            return "[x] Usuario: " + user + ", Acción: " + action + ", Mensaje: " + message + ", Enviado: " + send +
                   ", recivido en servidor: " + receive;
        }
    }
}
