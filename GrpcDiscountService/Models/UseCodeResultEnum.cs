namespace GrpcDiscountService.Models;

public enum UseCodeResultEnum
{
    Success = 0,
    InvalidInput = 1,
    NotFound = 2,
    AlreadyUsed = 3,
    Error = 4
}