namespace WildPay.Exceptions
{
    public class NullException : Exception
    {
        public NullException()
            : base("L'élément demandé n'existe pas dans la base de données.")
        {
        }

        public NullException(string message)
            : base(message)
        {
        }

        public NullException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
