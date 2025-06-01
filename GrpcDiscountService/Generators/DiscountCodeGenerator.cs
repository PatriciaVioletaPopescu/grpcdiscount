using NanoidDotNet;

namespace GrpcDiscountService.Generators
{
    public class DiscountCodeGenerator
    {
        public static HashSet<string> GenerateCodes(uint count, uint length)
        {
            var codes = new HashSet<string>();

            while (codes.Count < count)
            {
                var code = Nanoid.Generate(size: (int)length);
                codes.Add(code);
            }

            return codes;
        }
    }
}