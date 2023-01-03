namespace Userdatalib.Models;

public record UserdataModel
{
    [JsonRequired]
    public string? Name { get; set; }

    [JsonRequired]
    public int Age { get; set; }

    [JsonRequired]
    public int Score { get; set; }

    private string? _password;
    public string? Password
    {
        get => (string.IsNullOrEmpty(_password)) ? string.Empty : _password;
        set
        {
            var target = value ?? string.Empty;
            _password = (string.IsNullOrEmpty(value) || target.Length < 3) ? string.Empty : target;
        }
    }
}
