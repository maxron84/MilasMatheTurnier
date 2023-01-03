namespace Userdatalib.Repositories;

public interface IReflectable<T>
{
    T GetReflectedModel(Dictionary<string, object> properties);
}
