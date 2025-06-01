using GrpcDiscountService.Data;
using GrpcDiscountService.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcDiscountService.Repository
{
    public class DiscountRepository: IDiscountRepository
    {
        private readonly ILogger<DiscountRepository> _logger;
        private readonly DataContext _dbContext;

        public DiscountRepository(ILogger<DiscountRepository> logger, DataContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task AddRangeAsync(IList<Discount> discounts)
        {
            try
            {
                await _dbContext.Discounts.AddRangeAsync(discounts);
                _logger.LogInformation("Added {Count} discounts to context.", discounts.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding discounts to database.");
                throw;
            }
        }

        public Task<Discount?> GetByCodeAsync(string code)
        {
            try
            {
                return _dbContext.Discounts.FirstOrDefaultAsync(discount => discount.Code == code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving discount with code {Code}.", code);
                throw;
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Database changes saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes to database.");
                throw;
            }
        }

        public async Task<bool> TryUpdateAsync(Discount updated)
        {
            try
            {
                var result = await _dbContext.Discounts
                    .Where(discount => discount.Code == updated.Code && !discount.IsUsed)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(discount => discount.IsUsed, updated.IsUsed)
                        .SetProperty(discount => discount.UsedAt, updated.UsedAt)
                    );
                _logger.LogInformation("Database updates saved successfully.");

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating database.");
                throw;
            }
        }
    }
}
