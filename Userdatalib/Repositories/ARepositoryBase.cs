using Userdatalib.Repositories.Contracts;

namespace Userdatalib.Repositories;

public abstract class ARepositoryBase<T> : IRepository<T> where T : new()
{
    protected string? filePath;
    protected IList<T?>? models;
    public Type? ExceptionType { get; protected set; }
    public string? ExceptionMessage { get; protected set; }

    // GET, READ
    public Task<IList<T?>?> GetAllModels()
    {
        return Task.Run(() =>
        {
            return models;
        });
    }

    public Task<T?> GetModelByPropertyName(string propertyName, object propertyValue)
    {
        return Task.Run(() =>
        {
            if (models != null && models.Any())
                return models
                    .FirstOrDefault(x => typeof(T)
                    .GetProperty(propertyName)!
                    .GetValue(x)!
                    .Equals(propertyValue));
            return new T();
        });
    }

    // POST, CREATE
    public async Task AddModelAsync(T model)
    {
        models!.Add(model);
        await SaveToJsonFileAsync(filePath!, models!);
    }

    public async Task AddSpecifiedRangeOfModelsAsync(IList<Dictionary<string, object>> propertiesCollection)
    {
        await Task.Run(() =>
        {
            foreach (var properties in propertiesCollection)
                models!.Add(GetReflectedModel(properties));
        });
        await SaveToJsonFileAsync(filePath!, models!);
    }

    // PUT, UPDATE
    public async Task UpdateModelByPropertyAsync(Dictionary<string, object> properties, string keyProperty, string targetProperty)
    {
        T? existingModel = await GetModelByPropertyName(keyProperty, properties[keyProperty]);
        if (existingModel != null)
        {
            PropertyInfo? prop = existingModel.GetType().GetProperty(targetProperty);
            if (prop != null && prop.CanWrite)
                prop.SetValue(existingModel, properties[targetProperty], null);
            await SaveToJsonFileAsync(filePath!, models!);
        }
    }

    // DELETE, DELETE
    public async Task DeleteAllModelsAsync()
    {
        if (models!.Any())
            models!.Clear();
        await SaveToJsonFileAsync(filePath!, models!);
    }

    public async Task DeleteModelByPropertyAsync(string propertyName, object propertyValue)
    {
        var model = await GetModelByPropertyName(propertyName, propertyValue);
        if (model != null)
        {
            models!.Remove(model);
            await SaveToJsonFileAsync(filePath!, models!);
        }
    }

    // REPOSITORY HELPER
    protected T GetReflectedModel(Dictionary<string, object> properties)
    {
        T model = (T)Activator.CreateInstance(typeof(T))!;
        foreach (var property in properties)
        {
            PropertyInfo? targetProperty = model!.GetType().GetProperty(property.Key);
            if (targetProperty != null && targetProperty.CanWrite)
                targetProperty.SetValue(model, property.Value, null);
        }

        return model;
    }

    // JSON FILE INTERACTIONS
    protected async Task<List<T>> LoadFromJsonFileAsync(string jsonFilePath)
    {
        try
        {
            if (!File.Exists(jsonFilePath))
                throw new FileNotFoundException();

            using (StreamReader file = File.OpenText(jsonFilePath))
            {
                var deserialized = await Task<object?>.Run(() =>
                {
                    return new JsonSerializer().Deserialize(file, typeof(List<T>));
                });

                if (deserialized is null)
                    return new List<T>();

                return (List<T>)deserialized;
            }
        }
        catch (Exception ex)
        {
            ExceptionType = ex.GetType();
            ExceptionMessage = ex.Message;
            OnExceptionThrown(EventArgs.Empty);

            return new List<T>();
        }
    }

    protected async Task SaveToJsonFileAsync(string jsonFilePath, IList<T> collectionToSave)
    {
        try
        {
            using (StreamWriter file = File.CreateText(jsonFilePath))
            {
                await Task.Run(() =>
                {
                    new JsonSerializer().Serialize(file, collectionToSave);
                });
            }
        }
        catch (Exception ex)
        {
            ExceptionType = ex.GetType();
            ExceptionMessage = ex.Message;
            OnExceptionThrown(EventArgs.Empty);
        }
    }

    // EVENTS
    public event EventHandler? ExceptionThrown = delegate { };
    protected virtual void OnExceptionThrown(EventArgs e) => ExceptionThrown?.Invoke(this, e);
}
