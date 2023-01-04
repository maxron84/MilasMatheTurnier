namespace Userdatalib.Repositories.Contracts;

public interface IReflectable<T>
{
    T GetReflectedModel(Dictionary<string, object> properties);
}
