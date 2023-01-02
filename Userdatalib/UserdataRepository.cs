using Newtonsoft.Json;

namespace Userdatalib
{
    public class UserdataRepository
    {
        private string _jsonFilePath;
        private List<UserdataModel> _userDataModels;

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

        public async Task CreateVeryLargeExampleFileAsync()
        {
            var random = new Random();
            await Task.Run(() =>
            {
                for (int i = 0; i < 10_000_000; i++)
                {
                    _userDataModels.Add(new UserdataModel()
                    {
                        Name = $"BigDataUser_{i + 1}",
                        Age = random.Next(1, 101),
                        Score = random.Next(1, 10_000_001)
                    });
                }
            });
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
                return _userDataModels.Any(e => e.Name == name) ? _userDataModels.FirstOrDefault(e => e.Name == name) : new UserdataModel();
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
        public async Task DeleteAllUsersAsync()
        {
            _userDataModels.Clear();
            await SaveUserdataToJsonFileAsync();
        }

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
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"\n# ERROR: {ex.GetType()}: {ex.Message}\n");
                return new List<UserdataModel>();
            }
        }

        private async Task SaveUserdataToJsonFileAsync()
        {
            try
            {
                using (StreamWriter file = File.CreateText(_jsonFilePath))
                {
                    await Task.Run(() =>
                    {
                        new JsonSerializer().Serialize(file, _userDataModels);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n# ERROR: {ex.GetType()}: {ex.Message}\n");
            }
        }
    }
}
