namespace GrpcDiscountService.Validators
{
    public class GenerateRequestValidator
    {
        public static bool IsValid(uint count, uint length, out string? error)
        {
            if (count is <= 0 or > DiscountCodeConstants.MaxGenerateCount)
            {
                error = $"Count must be between 1 and {DiscountCodeConstants.MaxGenerateCount}.";
                return false;
            }

            if (!DiscountCodeConstants.ValidLengths.Contains(length))
            {
                error = $"Code length must be {string.Join(" or ", DiscountCodeConstants.ValidLengths)} characters.";
                return false;
            }

            error = null;
            return true;
        }
    }
}
