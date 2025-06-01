namespace GrpcDiscountService.Models
{
    public class Discount
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UsedAt { get; set; }
    }
}
