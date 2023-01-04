using Userdatalib.Models;
using Userdatalib.Repositories;

namespace Userfunctionlib;

public class Operator
{
    private string _userdataLocation;
    private string _userName;
    private List<UserdataModel?>? _sortedData;

    public Operator(string userdataLocation, string userName)
    {
        _userdataLocation = userdataLocation;
        _userName = userName;
    }

    // CONCRETE USE CASE OPERATIONS
    public async Task<string> GetAllUsersSortedByUserScoreDescAsync()
    {
        var data = await new UserdataRepo(_userdataLocation).GetAllModels();
        _sortedData = await Task<List<UserdataModel>>.Run(() =>
        {
            return data!
            .OrderByDescending(x => x!.Score)
            .ToList();
        });
        if (!_sortedData.Any())
            return "\n# Es sind noch keine Spieler eingetragen. Beginne jetzt mit einem neuen Spiel und sei der erste!\n";

        return string.Empty;
    }

    public async IAsyncEnumerable<string> GetEachUserdataModelReportAsync()
    {
        for (int i = 0; i < _sortedData!.Count(); i++)
        {
            await Task.Delay(1);
            yield return $"# {i + 1}.) Name: {_sortedData![i]!.Name} | Alter: {_sortedData![i]!.Age} | Punkte: {_sortedData![i]!.Score}";
        }
    }

    public async Task<bool> IsUserAlreadyExistingAsync(string userName)
    {
        var target = await GetUserdataModelByUserNameAsync(userName.ToLower());

        return target.Name != null;
    }

    public async Task<int> GetUserScoreByUserNameAsync(string userName)
    {
        var target = await GetUserdataModelByUserNameAsync(userName.ToLower());

        return target.Name != null ? target.Score : -1;
    }

    public async Task SetCurrentUserScoreByUserNameAsync(string userName, int bonus, int malus, bool equationPassed)
    {
        var target = GetUserdataModelByUserNameAsync(userName.ToLower()).Result;
        if (target.Name != null)
        {
            target.Score += equationPassed ? bonus : -malus;
            if (target.Score < 0)
                target.Score = 0;
            await new UserdataRepo(_userdataLocation).UpdateModelByPropertyAsync(GetObjectPropertiesLookup(target), nameof(target.Name), nameof(target.Score));
        }
    }

    public async Task<string?> GetUserPasswordAsync(string userName)
    {
        var target = await GetUserdataModelByUserNameAsync(userName.ToLower());

        return target.Password;
    }

    public async Task CreateNewUserAsync(string userName, int userAge, string userPassword)
    {
        await new UserdataRepo(_userdataLocation)
            .AddModelAsync(new UserdataModel()
            {
                Name = userName.ToLower(),
                Age = userAge,
                Score = 0,
                Password = userPassword
            });
    }

    public async Task UpdateUserAgeByUserNameAsync(string userName, int userAge)
    {
        var target = GetUserdataModelByUserNameAsync(userName.ToLower()).Result;
        if (target.Name != null)
        {
            target.Age = userAge;
            await new UserdataRepo(_userdataLocation).UpdateModelByPropertyAsync(GetObjectPropertiesLookup(target), nameof(target.Name), nameof(target.Age));
        }
    }

    public async Task CreateExampleWithBigDataAsync()
    {
        var random = new Random();
        var maximum = 10_000_000;
        var propertiesCollection = new List<Dictionary<string, object>>();
        for (int i = 0; i < maximum; i++)
        {
            propertiesCollection.Add(new Dictionary<string, object>
            {
                { "Name", $"Bigdatauser_{i}" },
                { "Age", random.Next(1, maximum / 10_000 + 1) },
                { "Score", random.Next(1, maximum) }
            });
        }
        await new UserdataRepo(_userdataLocation).AddSpecifiedRangeOfModelsAsync(propertiesCollection);
    }

    public async Task DeleteAllUsersAsync()
    {
        await new UserdataRepo(_userdataLocation).DeleteAllModelsAsync();
    }

    // HELPER
    private async Task<UserdataModel> GetUserdataModelByUserNameAsync(string userName)
    {
        return await new UserdataRepo(_userdataLocation).GetModelByPropertyName("Name", userName.ToLower()) ?? new UserdataModel();
    }

    private Dictionary<string, object> GetObjectPropertiesLookup(object target)
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        PropertyInfo[] properties = target.GetType().GetProperties();
        foreach (PropertyInfo property in properties)
            result.Add(property.Name, property.GetValue(target, null)!);

        return result;
    }
}
