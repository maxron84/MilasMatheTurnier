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
        get
        {
            if (string.IsNullOrEmpty(_password))
                return string.Empty;
            return _password;
        }
        set
        {
            var target = value ?? string.Empty;

            if (string.IsNullOrEmpty(value) || target.Length < 3)
                _password = string.Empty;
            _password = target;
        }
    }
}
