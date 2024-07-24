namespace WildPay.Models.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int GroupId { get; set; }
        public Group? Group { get; set; }
    }
}
