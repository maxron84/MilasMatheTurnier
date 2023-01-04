namespace Userdatalib.Repositories;

public abstract class ARepositoryBase<T> : IRepositible<T> where T : new()
{
    protected string? filePath;
    protected IList<T?>? models;

    // GET, READ
    public Task<IList<T?>?> GetAllModels()
    {
        return Task.Run(() =>
        {
            return models;
        });
    }

    public Task<T?> GetModelByProperty(object property)
    {
        return Task.Run(() =>
        {
            if (models!.Any(e => e!.GetType().GetProperty(nameof(property))!.GetValue(e, null) == property))
                return models!.FirstOrDefault(e => e!.GetType().GetProperty(nameof(property))!.GetValue(e, null) == property);
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
    public async Task UpdateModelByPropertyAsync(Dictionary<string, object> properties, string targetProp)
    {
        var targetProperty = properties.FirstOrDefault().Key;
        T? existingModel = await GetModelByProperty(targetProperty)!;
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
        if (models!.Count() > 0)
            models!.Clear();
        await SaveToJsonFileAsync(filePath!, models!);
    }

    public async Task DeleteModelByPropertyAsync(object property)
    {
        var model = await GetModelByProperty(property)!;
        if (model != null)
        {
            models!.Remove(model);
            await SaveToJsonFileAsync(filePath!, models!);
        }
    }

    // REPOSITORY HELPER
    public T GetReflectedModel(Dictionary<string, object> properties)
    {
        T? model = (T?)Activator.CreateInstance(typeof(T?));
        foreach (var property in properties)
        {
            PropertyInfo? prop = model!.GetType().GetProperty(property.Key);
            if (prop != null && prop.CanWrite)
                prop.SetValue(model, property.Value, null);
        }

        return model ?? new T();
    }

    // JSON FILE INTERACTIONS
    public async Task<IList<T>> LoadFromJsonFileAsync(string jsonFilePath)
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
            Console.WriteLine($"\n# {ex.GetType()}: {ex.Message}\n");
            return new List<T>();
        }
    }

    public async Task SaveToJsonFileAsync(string jsonFilePath, IList<T> collectionToSave)
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
            Console.WriteLine($"\n# {ex.GetType()}: {ex.Message}\n");
        }
    }
}
