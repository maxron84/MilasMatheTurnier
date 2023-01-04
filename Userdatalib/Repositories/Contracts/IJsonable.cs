namespace Userdatalib.Repositories.Contracts;

public interface IJsonable<T>
{
    Task<List<T>> LoadFromJsonFileAsync(string jsonFilePath);

    Task SaveToJsonFileAsync(string jsonFilePath, IList<T> collectionToSave);
}
