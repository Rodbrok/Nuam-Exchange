namespace NuamExchange.Application.Exceptions;

public class NotFoundException(string message) : Exception(message);

public class ConflictException(string message) : Exception(message);

public class ConcurrencyException(string message) : Exception(message);

public sealed class ValidationException(string message, IReadOnlyDictionary<string, string[]> errors) : Exception(message)
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors;
}
