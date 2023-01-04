namespace Userdatalib.Repositories;

public interface ICrudable<T>
{
    // GET, READ
    Task<IList<T?>?> GetAllModels();
    Task<T?> GetModelByProperty(object property);

    // POST, CREATE
    Task AddModelAsync(T model);
    Task AddSpecifiedRangeOfModelsAsync(IList<Dictionary<string, object>> propertiesCollection);

    // PUT, UPDATE
    Task UpdateModelByPropertyAsync(Dictionary<string, object> properties, string targetProp);

    // DELETE, DELETE
    Task DeleteAllModelsAsync();
    Task DeleteModelByPropertyAsync(object property);
}
