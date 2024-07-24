namespace WildPay.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
    }
}
