namespace Userdatalib.Repositories;

public interface IJsonable<T>
{
    Task<IList<T>> LoadFromJsonFileAsync(string jsonFilePath);

    Task SaveToJsonFileAsync(string jsonFilePath, IList<T> collectionToSave);
}
