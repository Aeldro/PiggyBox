namespace WildPay.Models.Entities
{
    public class Expenditure
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
