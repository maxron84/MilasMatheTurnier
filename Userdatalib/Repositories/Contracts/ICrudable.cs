namespace Userdatalib.Repositories;

public interface ICrudable<T>
{
    // GET, READ
    Task<IList<T?>?> GetAllModels();
    Task<T?> GetModelByProperty(object property);

    // POST, CREATE
    Task AddModelAsync(T model);
    Task AddSpecifiedRangeOfModelsAsync(Dictionary<string, object> properties, int maximum);

    // PUT, UPDATE
    Task UpdateModelByPropertyAsync(Dictionary<string, object> properties, string targetProp);

    // DELETE, DELETE
    Task DeleteAllModelsAsync();
    Task DeleteModelByPropertyAsync(object property);
}
