using Userdatalib;

namespace Userfunctionlib;

public class Operator
{
    private string _userdataLocation;
    private string _userName;
    private List<UserdataModel>? _sortedData;

    public Operator(string userdataLocation, string userName)
    {
        _userdataLocation = userdataLocation;
        _userName = userName;
    }

    // CONCRETE USE CASE OPERATIONS
    public async Task<string> GetAllUsersSortedByUserScoreDescAsync()
    {
        var data = await new Userdatalib.UserdataRepository(_userdataLocation).GetAllUsers();
        _sortedData = await Task<List<UserdataModel>>.Run(() =>
        {
            return data
            .OrderByDescending(x => x.Score)
            .ToList();
        });
        if (_sortedData.Count() < 1)
            return "\n# Es sind noch keine Spieler eingetragen. Beginne jetzt mit einem neuen Spiel und sei der erste!\n";

        return string.Empty;
    }

    public async IAsyncEnumerable<string> GetEachUserdataModelReportAsync()
    {
        for (int i = 0; i < _sortedData!.Count(); i++)
        {
            await Task.Delay(1);
            yield return $"# {i + 1}.) Name: {_sortedData![i].Name} | Alter: {_sortedData[i].Age} | Punkte: {_sortedData[i].Score}";
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
        }
        await new Userdatalib.UserdataRepository(_userdataLocation).UpdateUserByNameAsync(target);
    }

    public async Task<string?> GetUserPasswordAsync(string userName)
    {
        var target = await GetUserdataModelByUserNameAsync(userName.ToLower());

        return target.Password;
    }

    public async Task CreateNewUserAsync(string userName, int userAge, string userPassword)
    {
        await new Userdatalib.UserdataRepository(_userdataLocation)
            .CreateUserAsync(new UserdataModel()
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
        target.Age = userAge;
        await new Userdatalib.UserdataRepository(_userdataLocation).UpdateUserByNameAsync(target);
    }

    public async Task CreateExampleWithBigDataAsync() => await new Userdatalib.UserdataRepository(_userdataLocation).CreateVeryLargeExampleFileAsync();

    public async Task DeleteAllUsersAsync() => await new Userdatalib.UserdataRepository(_userdataLocation).DeleteAllUsersAsync();

    // HELPER
    private async Task<UserdataModel> GetUserdataModelByUserNameAsync(string userName)
    {
        return await new Userdatalib.UserdataRepository(_userdataLocation).GetUserByName(userName.ToLower())! ?? new UserdataModel();
    }
}
