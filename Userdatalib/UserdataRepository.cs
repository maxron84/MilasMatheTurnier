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
            _userDataModels = LoadUserdataFromJsonFile();
        }

        // CREATE, POST
        public Task CreateUser(UserdataModel userdataModel)
        {
            _userDataModels.Add(userdataModel);
            SaveUserdataToJsonFile();

            return Task.CompletedTask;
        }

        // READ, GET
        public List<UserdataModel>? GetAllUsers() => _userDataModels;

        public UserdataModel? GetUserByName(string name) => _userDataModels.FirstOrDefault(e => e.Name == name) ?? new UserdataModel();

        // UPDATE, PUT
        public void UpdateUserByName(UserdataModel userdataModel)
        {
            var existingUserdataModel = GetUserByName(userdataModel.Name!);
            if (existingUserdataModel != null)
            {
                existingUserdataModel.Name = userdataModel.Name;
                existingUserdataModel.Age = userdataModel.Age;
                existingUserdataModel.Score = userdataModel.Score;
                existingUserdataModel.Password = userdataModel.Password;
                SaveUserdataToJsonFile();
            }
        }

        // DELETE, DELETE
        public Task DeleteUserByName(string name)
        {
            var userdataModel = GetUserByName(name);
            if (userdataModel != null)
            {
                _userDataModels.Remove(userdataModel);
                SaveUserdataToJsonFile();
            }

            return Task.CompletedTask;
        }

        // JSON HELPER
        private List<UserdataModel> LoadUserdataFromJsonFile()
        {
            if (!File.Exists(_jsonFilePath))
                throw new FileNotFoundException();

            using (StreamReader file = File.OpenText(_jsonFilePath))
            {
                var serializer = new JsonSerializer();
                var deserialized = serializer.Deserialize(file, typeof(List<UserdataModel>));
                if (deserialized is null)
                    return new List<UserdataModel>();

                return (List<UserdataModel>)deserialized;
            }
        }

        private Task SaveUserdataToJsonFile()
        {
            using (StreamWriter file = File.CreateText(_jsonFilePath))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, _userDataModels);
            }

            return Task.CompletedTask;
        }
    }
}
