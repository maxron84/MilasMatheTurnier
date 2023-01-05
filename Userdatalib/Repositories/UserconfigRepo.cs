using Userdatalib.Models;

namespace Userdatalib.Repositories;

public class UserconfigRepo : ARepositoryBase<UserconfigModel>
{
    public UserconfigRepo(string filePath)
    {
        this.filePath = filePath;
        models = LoadFromJsonFileAsync().Result!;
    }
}
