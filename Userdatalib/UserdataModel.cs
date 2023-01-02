using System.ComponentModel.DataAnnotations;

namespace Userdatalib;

public record UserdataModel
{
    [Required]
    public string? Name { get; set; }

    [Required]
    public int Age { get; set; }

    [Required]
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
