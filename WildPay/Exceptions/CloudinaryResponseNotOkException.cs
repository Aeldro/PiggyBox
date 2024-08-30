namespace WildPay.Exceptions
{
    public class CloudinaryResponseNotOkException : Exception
    {
        public CloudinaryResponseNotOkException(string message)
            : base(message) { }
    }
}
