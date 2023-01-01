using Newtonsoft.Json;

namespace Userdatalib
{
    public class UserdataRepository
    {
        public List<UserdataModel> UserDataModels { get; private set; }
        private string _jsonFilePath;

        public UserdataRepository(string jsonFilePath)
        {
            _jsonFilePath = jsonFilePath;
            UserDataModels = LoadUserdataFromJsonFileAsync().Result;
        }

        // CREATE, POST
        public async Task CreateUserAsync(UserdataModel userdataModel)
        {
            UserDataModels.Add(userdataModel);
            await SaveUserdataToJsonFileAsync();
        }

        public async Task CreateVeryLargeExampleFileAsync()
        {
            var random = new Random();
            Console.WriteLine("# DEBUG: BIG DATA EXAMPLE FILE CREATION STARTED...");
            await Task.Run(() =>
            {
                for (int i = 0; i < 10_000_000; i++)
                {
                    UserDataModels.Add(new UserdataModel()
                    {
                        Name = $"BigDataUser_{i + 1}",
                        Age = random.Next(1, 101),
                        Score = random.Next(1, 10_000_001)
                    });
                }
            });
            await SaveUserdataToJsonFileAsync();
            Console.WriteLine("# DEBUG: ALL TESTUSERS SUCCESSFULLY ADDED TO JSON FILE!");
        }

        // READ, GET
        public Task<List<UserdataModel>> GetAllUsers()
        {
            return Task.Run(() =>
            {
                return UserDataModels;
            });
        }

        public Task<UserdataModel?>? GetUserByName(string name)
        {
            return Task.Run(() =>
            {
                return UserDataModels.Any(e => e.Name == name) ? UserDataModels.FirstOrDefault(e => e.Name == name) : null;
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
            UserDataModels.Clear();
            await SaveUserdataToJsonFileAsync();
            Console.WriteLine("# DEBUG: ALL DATA SUCCESSFULLY DELETED FROM JSON FILE!");
        }

        public async Task DeleteUserByNameAsync(string name)
        {
            var userdataModel = await GetUserByName(name)!;
            if (userdataModel != null)
            {
                UserDataModels.Remove(userdataModel);
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
                        new JsonSerializer().Serialize(file, UserDataModels);
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
