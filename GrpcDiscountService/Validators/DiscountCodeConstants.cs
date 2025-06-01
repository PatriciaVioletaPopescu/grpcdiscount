namespace GrpcDiscountService.Validators
{
    public static class DiscountCodeConstants
    {
        public static readonly uint[] ValidLengths = { 7, 8 };
        public const ushort MaxGenerateCount = 2000;
    }
}
