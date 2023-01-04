namespace Userdatalib.Repositories;

public interface ICrudable<T>
{
    // GET, READ
    Task<IList<T?>?> GetAllModels();
    Task<T?> GetModelByPropertyName(string propertyName, object propertyValue);

    // POST, CREATE
    Task AddModelAsync(T model);
    Task AddSpecifiedRangeOfModelsAsync(IList<Dictionary<string, object>> propertiesCollection);

    // PUT, UPDATE
    Task UpdateModelByPropertyAsync(Dictionary<string, object> properties, string keyProperty, string targetProperty);

    // DELETE, DELETE
    Task DeleteAllModelsAsync();
    Task DeleteModelByPropertyAsync(string propertyName, object propertyValue);
}
