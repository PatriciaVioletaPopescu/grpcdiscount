using Grpc.Core;
using GrpcDiscountService.Generators;
using GrpcDiscountService.Models;
using GrpcDiscountService.Repository;
using GrpcDiscountService.Validators;

namespace GrpcDiscountService.Services
{
    public class DiscounterService : Discounter.DiscounterBase
    {
        private readonly ILogger<DiscounterService> _logger;
        private readonly IDiscountRepository _discountRepository;

        public DiscounterService(IDiscountRepository discountRepository, ILogger<DiscounterService> logger)
        {
            _logger = logger;
            _discountRepository = discountRepository;
        }

        public override async Task<GenerateReply> Generate(GenerateRequest request, ServerCallContext context)
        {
            try
            {
                if (!GenerateRequestValidator.IsValid(request.Count, request.Length, out var error))
                {
                    _logger.LogWarning("Invalid generate request: {Error}", error);
                    return new GenerateReply { Result = false };
                }

                _logger.LogInformation("Generating {Count} codes with length {Length}", request.Count, request.Length);
                var discounts = CreateDiscounts(request.Count, request.Length);

                await _discountRepository.AddRangeAsync(discounts);
                await _discountRepository.SaveChangesAsync();

                _logger.LogInformation("Successfully generated and saved {Count} codes", discounts.Count);
                return new GenerateReply { Result = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during Generate");
                return new GenerateReply { Result = false };
            }
        }

        public override async Task<UseCodeReply> UseCode(UseCodeRequest request, ServerCallContext context)
        {
            try
            {
                var code = request.Code.Trim();
                if (!DiscountCodeValidator.IsValid(code, out var error))
                {
                    _logger.LogWarning("Invalid code used: {Error}", error);
                    return new UseCodeReply { Result = (uint)UseCodeResultEnum.InvalidInput };
                }

                var result = await MarkCodeAsUsed(code);
                return new UseCodeReply { Result = (uint)result };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during UseCode");
                return new UseCodeReply { Result = (uint)UseCodeResultEnum.Error };
            }
            
        }

        private List<Discount> CreateDiscounts(uint count, uint length)
        {
            var codes = DiscountCodeGenerator.GenerateCodes(count, length);
            return codes.Select(code => new Discount
            {
                Code = code,
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            }).ToList();
        }

        private async Task<UseCodeResultEnum> MarkCodeAsUsed(string code)
        {
            var discount = await _discountRepository.GetByCodeAsync(code);
            if (discount == null)
            {
                _logger.LogInformation("Discount with code {Code} not found.", code);
                return UseCodeResultEnum.NotFound;
            }

            if (discount.IsUsed)
            {
                _logger.LogInformation("Discount with code {Code} already used at {UsedAt}.", code, discount.UsedAt);
                return UseCodeResultEnum.AlreadyUsed;
            }

            discount.IsUsed = true;
            discount.UsedAt = DateTime.UtcNow;
            var result = await _discountRepository.TryUpdateAsync(discount);

            if (result == false)
            {
                _logger.LogInformation("Discount with code {Code} already used.", code);
                return UseCodeResultEnum.AlreadyUsed;
            }

            _logger.LogInformation("Code successfully used: {Code}", code);
            return UseCodeResultEnum.Success;
        }
    }
}
