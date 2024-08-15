namespace WildPay.Exceptions
{

    public class DatabaseException : Exception
    {
        public DatabaseException()
            : base("La base de données est temporairement indisponible.")
        {
        }

        public DatabaseException(string message)
    : base(message)
        {
        }

        public DatabaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
