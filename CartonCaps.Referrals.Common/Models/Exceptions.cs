namespace CartonCaps.Referrals.Common.Models
{
    public class RateLimitExceededException(string message) : Exception(message)
    {
    }

    public class VendorUnavailableException(string message) : Exception(message)
    {
    }

    public class NotFoundException(string message) : Exception(message)
    {
    }

    public class ConflictException(string message) : Exception(message)
    {
    }

    public class MissingIdempotencyException(string message) : Exception(message)
    {
    }
}
