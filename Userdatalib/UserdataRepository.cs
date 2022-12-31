using Newtonsoft.Json;

namespace Userdatalib
{
    public class UserdataRepository
    {
        private List<UserdataModel> _userDataModels;
        private string _jsonFilePath;

        public UserdataRepository(string jsonFilePath)
        {
            _jsonFilePath = jsonFilePath;
            _userDataModels = LoadUserdataFromJsonFileAsync().Result;
        }

        // CREATE, POST
        public async Task CreateUserAsync(UserdataModel userdataModel)
        {
            _userDataModels.Add(userdataModel);
            await SaveUserdataToJsonFileAsync();
        }

        // READ, GET
        public Task<List<UserdataModel>> GetAllUsers()
        {
            return Task.Run(() =>
            {
                return _userDataModels;
            });
        }

        public Task<UserdataModel?>? GetUserByName(string name)
        {
            return Task.Run(() =>
            {
                return _userDataModels.Any(e => e.Name == name) ? _userDataModels.FirstOrDefault(e => e.Name == name) : null;
            });
        }

        // UPDATE, PUT
        public async Task UpdateUserByNameAsync(UserdataModel userdataModel)
        {
            var targetName = userdataModel.Name ?? string.Empty;
            var existingUserdataModel = await GetUserByName(targetName)!;
            if (existingUserdataModel != null)
            {
                existingUserdataModel.Name = userdataModel.Name;
                existingUserdataModel.Age = userdataModel.Age;
                existingUserdataModel.Score = userdataModel.Score;
                existingUserdataModel.Password = userdataModel.Password;
                await SaveUserdataToJsonFileAsync();
            }
        }

        // DELETE, DELETE
        public async Task DeleteUserByNameAsync(string name)
        {
            var userdataModel = await GetUserByName(name)!;
            if (userdataModel != null)
            {
                _userDataModels.Remove(userdataModel);
                await SaveUserdataToJsonFileAsync();
            }
        }

        // JSON HELPER
        private async Task<List<UserdataModel>> LoadUserdataFromJsonFileAsync()
        {
            if (!File.Exists(_jsonFilePath))
                throw new FileNotFoundException();

            using (StreamReader file = File.OpenText(_jsonFilePath))
            {
                var deserialized = await Task<object?>.Run(() =>
                {
                    return new JsonSerializer().Deserialize(file, typeof(List<UserdataModel>));
                });

                if (deserialized is null)
                    return new List<UserdataModel>();

                return (List<UserdataModel>)deserialized;
            }
        }

        private async Task SaveUserdataToJsonFileAsync()
        {
            using (StreamWriter file = File.CreateText(_jsonFilePath))
            {
                await Task.Run(() =>
                {
                    new JsonSerializer().Serialize(file, _userDataModels);
                });
            }
        }
    }
}
