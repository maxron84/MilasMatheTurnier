namespace Userdatalib.Models;

public record UserconfigModel
{
    [JsonRequired]
    public Dictionary<int, (int bonus, int malus, List<string> allowedOperators)>? UserSetupLookup { get; set; }

    [JsonRequired]
    public Dictionary<int, int>? UserTimerLookup { get; set; }
}
