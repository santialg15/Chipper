namespace LogServer.Exceptions
{
    public class LogDoesNotExistException : Exception
    {
        public LogDoesNotExistException()
            : base("El log no existe")
        {
        }
    }
}