using Userdatalib.Models;

namespace Userdatalib.Repositories;

public class UserdataRepo : ARepositoryBase<UserdataModel>
{
    public UserdataRepo(string filePath)
    {
        this.filePath = filePath;
        models = LoadFromJsonFileAsync(this.filePath).Result!;
    }
}
