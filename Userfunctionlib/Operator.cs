using System.Text;
using Userdatalib;

namespace Userfunctionlib;

public class Operator
{
    public List<UserdataModel>? SortedData { get; private set; }
    private string _userdataLocation;
    private string _userName;

    public Operator(string userdataLocation, string userName)
    {
        _userdataLocation = userdataLocation;
        _userName = userName;
    }

    // CONCRETE USE CASE OPERATIONS
    public async Task<string> GetAllUsersUserNameUserScoreAsync()
    {
        var data = await new Userdatalib.UserdataRepository(_userdataLocation).GetAllUsers();
        SortedData = await Task<List<UserdataModel>>.Run(() =>
        {
            return data
            .OrderByDescending(x => x.Score)
            .ToList();
        });
        if (SortedData.Count() < 1)
            return "\n# Es sind noch keine Spieler eingetragen. Beginne jetzt mit einem neuen Spiel und sei der erste!\n\n";

        return string.Empty;
    }

    public async IAsyncEnumerable<string> GetEachUserDataModelReportAsync()
    {
        for (int i = 0; i < SortedData!.Count(); i++)
        {
            await Task.Delay(0);
            yield return $"# {i + 1}.) Name: {SortedData![i].Name} | Alter: {SortedData[i].Age} | Punkte: {SortedData[i].Score}";
        }
    }

    public bool IsUserAlreadyExisting(string userName)
    {
        var target = GetUserdataModelByUserNameAsync(userName.ToLower()).Result;

        return target.Name != null;
    }

    public int GetUserScoreByUserName(string userName)
    {
        var target = GetUserdataModelByUserNameAsync(userName.ToLower()).Result;

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

    public string? GetUserPassword(string userName)
    {
        var target = GetUserdataModelByUserNameAsync(userName.ToLower()).Result;

        return target.Password;
    }

    public Task CreateNewUser(string userName, int userAge, string userPassword)
    {
        _ = new Userdatalib.UserdataRepository(_userdataLocation)
            .CreateUserAsync(new UserdataModel()
            {
                Name = userName.ToLower(),
                Age = userAge,
                Score = 0,
                Password = userPassword
            });

        return Task.CompletedTask;
    }

    public Task UpdateUserAgeByUserName(string userName, int userAge)
    {
        var target = GetUserdataModelByUserNameAsync(userName.ToLower()).Result;
        target.Age = userAge;
        _ = new Userdatalib.UserdataRepository(_userdataLocation).UpdateUserByNameAsync(target);

        return Task.CompletedTask;
    }

    public async Task CreateExampleWithBigData() => await new Userdatalib.UserdataRepository(_userdataLocation).CreateVeryLargeExampleFileAsync();

    public async Task DeleteAllUsersAsync() => await new Userdatalib.UserdataRepository(_userdataLocation).DeleteAllUsersAsync();

    // HELPER
    private async Task<UserdataModel> GetUserdataModelByUserNameAsync(string userName)
    {
        return await new Userdatalib.UserdataRepository(_userdataLocation).GetUserByName(userName.ToLower())! ?? new UserdataModel();
    }
}
