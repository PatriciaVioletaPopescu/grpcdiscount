namespace GrpcDiscountService.Validators
{
    public static class DiscountCodeValidator
    {
        public static bool IsValid(string? code, out string? error)
        {
            if (string.IsNullOrEmpty(code))
            {
                error = "Code cannot be empty.";
                return false;
            }

            if (!DiscountCodeConstants.ValidLengths.Contains((uint)code.Length))
            {
                error = $"Code length must be {string.Join(" or ", DiscountCodeConstants.ValidLengths)} characters.";
                return false;
            }

            error = null;
            return true;
        }
    }
}
