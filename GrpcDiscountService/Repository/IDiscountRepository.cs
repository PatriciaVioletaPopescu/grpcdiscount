using GrpcDiscountService.Models;

namespace GrpcDiscountService.Repository
{
    public interface IDiscountRepository
    {
        Task AddRangeAsync(IList<Discount> discounts);
        Task<Discount?> GetByCodeAsync(string code);
        Task SaveChangesAsync();
        Task<bool> TryUpdateAsync(Discount updated);
    }
}
