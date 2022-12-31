using System.Text;
using Userdatalib;

namespace Userfunctionlib;

public class Operator
{
    private string _userdataLocation;
    private string _userName;
    private StringBuilder _stringBuilder;

    public Operator(string userdataLocation, string userName)
    {
        _userdataLocation = userdataLocation;
        _userName = userName;
        _stringBuilder = new();
    }

    // CONCRETE USE CASE OPERATIONS
    public async Task<string> GetAllUsersUserNameUserScoreAsync()
    {
        // System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1+AsyncStateMachineBox`1[System.String,Userfunctionlib.Operator+<GetAllUsersUserNameUserScore>d__4]
        var data = await new Userdatalib.UserdataRepository(_userdataLocation).GetAllUsers();
        var target = data
            .OrderByDescending(x => x.Score)
            .ToList();
        if (target.Count() < 1)
            return "\n# Es sind noch keine Spieler eingetragen. Beginne jetzt mit einem neuen Spiel und sei der erste!\n\n";
        _stringBuilder.Clear();
        _stringBuilder.Append("# Die besten Spieler:\n\n");
        for (int i = 0; i < target.Count(); i++)
        {
            _stringBuilder.Append($"# {i + 1}.) ");
            _stringBuilder.Append($"Name: {target[i].Name} | ");
            _stringBuilder.Append($"Alter: {target[i].Age} | ");
            _stringBuilder.Append($"Punkte: {target[i].Score}\n");
        }
        _stringBuilder.Append("\n");

        return _stringBuilder.ToString();
    }

    public bool IsUserAlreadyExisting(string userName)
    {
        var target = GetUserdataModelByUserNameAsync(userName).Result;

        return target.Name != null;
    }

    public int GetUserScoreByUserName(string userName)
    {
        var target = GetUserdataModelByUserNameAsync(userName.ToLower()).Result;

        return target.Name != null ? target.Score : -1;
    }

    public Task SetCurrentUserScoreByUserName(string userName, int bonus, int malus, bool equationPassed)
    {
        var target = GetUserdataModelByUserNameAsync(userName.ToLower()).Result;
        if (target.Name != null)
        {
            target.Score += equationPassed ? bonus : -malus;
            if (target.Score < 0)
                target.Score = 0;
        }
        _ = new Userdatalib.UserdataRepository(_userdataLocation).UpdateUserByNameAsync(target);

        return Task.CompletedTask;
    }

    public string? GetUserPassword(string userName)
    {
        var target = GetUserdataModelByUserNameAsync(userName.ToLower()).Result;

        return target.Password;
    }

    public Task CreateNewUser(string userName, int userAge, string userPassword)
    {
        _ = new Userdatalib.UserdataRepository(_userdataLocation).CreateUserAsync(new UserdataModel() { Name = userName.ToLower(), Age = userAge, Score = 0, Password = userPassword });

        return Task.CompletedTask;
    }

    public Task UpdateUserAgeByUserName(string userName, int userAge)
    {
        var target = GetUserdataModelByUserNameAsync(userName.ToLower()).Result;
        target.Age = userAge;
        _ = new Userdatalib.UserdataRepository(_userdataLocation).UpdateUserByNameAsync(target);

        return Task.CompletedTask;
    }

    // HELPER
    private async Task<UserdataModel> GetUserdataModelByUserNameAsync(string userName)
    {
        return await new Userdatalib.UserdataRepository(_userdataLocation).GetUserByName(userName.ToLower())! ?? new UserdataModel();
    }
}
