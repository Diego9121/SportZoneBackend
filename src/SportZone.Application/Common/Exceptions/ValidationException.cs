namespace SportZone.Application.Common.Exceptions;

// Se lanza cuando una regla de negocio no se cumple (ej. email duplicado). El middleware la traduce a 400.
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }
}
