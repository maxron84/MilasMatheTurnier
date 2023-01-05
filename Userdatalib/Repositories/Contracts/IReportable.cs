namespace Userdatalib.Repositories.Contracts;

public interface IReportable
{
    Type? ExceptionType { get; }
    string? ExceptionMessage { get; }
    event EventHandler? ExceptionThrown;
}
