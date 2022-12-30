﻿using System.Reflection;
using System.Text;
using Userfunctionlib;

var userName = string.Empty;
(int Min, int Max) userNameSize = (3, 12);
var userAge = 0;
var userScore = 0;
var bonus = 0;
var bonusOpenEnd = 16;
var malus = 0;
var malusOpenEnd = 4;
var userSetupLookup = new Dictionary<int, (int bonus, int malus, List<string> allowedOperators)>
{
    { 6, (2, 1, new List<string> { "+", "-" }) },
    { 7, (4, 1, new List<string> { "+", "-", "*" }) },
    { 8, (8, 2, new List<string> { "+", "-", "*", "/" }) },
    { 9, (bonusOpenEnd, malusOpenEnd, new List<string> { "+", "-", "*", "/" }) }
};
var outputFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? @".";
var userdataLocation = Path.Combine(outputFolder, "Userdata.json");
var userInput = string.Empty;
Validator validator;
var stringBuilder = new StringBuilder();
var userEquation = 0.0;
var equationPassed = false;

beginIntroduction:
Console.WriteLine("# Gebe 1 ein um ein neues Spiel zu beginnen, oder gebe 2 ein um die Bestenliste anzuzeigen. Gebe 3 ein, um die Konsole aufzuräumen.");
userInput = Console.ReadLine()!;
if (userInput != "1" && userInput != "2" && userInput != "3")
    goto beginIntroduction;
if (userInput == "2")
{
    Console.Write(new Operator(userdataLocation, userName).GetAllUsersUserNameUserScore());
    goto beginIntroduction;
}
else if (userInput == "3")
{
    Console.Clear();
    goto beginIntroduction;
}
else
{
    Console.WriteLine("# Dein Name?");
}
while (true)
{
    userName = Console.ReadLine();
    if (!string.IsNullOrEmpty(userName) && !string.IsNullOrWhiteSpace(userName) && userName.Length >= userNameSize.Min && userName.Length <= userNameSize.Max)
        break;
    Console.WriteLine($"# Bitte einen Namen mit einer Länge von {userNameSize.Min} bis {userNameSize.Max} Zeichen eingeben!");
}

Console.WriteLine("# Dein Alter?");
beginAgeValidation:
while (!Int32.TryParse(Console.ReadLine(), out userAge))
{
    if (Int32.TryParse(Console.ReadLine(), out userAge) && userAge < 6)
    {
        Console.WriteLine("# Du musst mindestens 6 Jahre alt sein!");
        goto beginAgeValidation;
    }
    Console.WriteLine("# Bitte nur Zahlen eingeben!");
}

bonus = userSetupLookup.ContainsKey(userAge) ? userSetupLookup[userAge].bonus : bonusOpenEnd;
malus = userSetupLookup.ContainsKey(userAge) ? userSetupLookup[userAge].malus : malusOpenEnd;

if (new Operator(userdataLocation, userName).IsUserAlreadyExisting(userName))
{
    var targetPassword = new Operator(userdataLocation, userName).GetUserPassword(userName);
    if (!string.IsNullOrEmpty(targetPassword))
    {
        Console.WriteLine("# Bitte gebe dein Passwort ein:");
        while (GetPlaintextPasswordByMaskedInput() != targetPassword)
            Console.WriteLine("# Das eingegebene Passwort ist falsch!");
    }
    _ = new Operator(userdataLocation, userName).UpdateUserAgeByUserName(userName, userAge);
    userScore = new Operator(userdataLocation, userName).GetUserScoreByUserName(userName);
    Console.WriteLine($"# Willkommen zurück, {userName}! Dein aktueller Punktestand lautet: {userScore}");
}
else
{
    var userPassword = string.Empty;
    Console.WriteLine("# Wähle ein Passwort aus mindestens 3 Zeichen oder drücke einfach Enter wenn du kein Passwort setzen möchtest:");
    userInput = GetPlaintextPasswordByMaskedInput();
    if (!string.IsNullOrEmpty(userInput))
        userPassword = userInput;
    _ = new Operator(userdataLocation, userName).CreateNewUser(userName, userAge, userPassword);
    Console.WriteLine($"# Willkommen, {userName}! Du bist also {userAge} Jahre alt und beginnst daher mit dem Schwierigkeitsgrad {malus}. Viel Spaß!");
}

beginOperationValidation:
while (true)
{
    validator = new(userAge);
    var allowedOperatorsOutput = userAge < 9 ? userSetupLookup[userAge].allowedOperators : userSetupLookup[9].allowedOperators;
    stringBuilder.Clear();
    allowedOperatorsOutput.ForEach(x => stringBuilder.Append(x + " "));
    Console.WriteLine($"# Wähle eine der {allowedOperatorsOutput.Count()} Grundrechenarten: {stringBuilder.ToString()}oder drücke ENTER zum beenden deiner Sitzung.");
    userInput = Console.ReadLine();
    if (userInput == "")
        goto beginIntroduction;
    if (allowedOperatorsOutput.Any(x => userInput == x))
    {
        Console.Write(validator.GetUsertaskReport(userInput!));
        while (!Double.TryParse(Console.ReadLine(), out userEquation))
            Console.WriteLine("# Bitte nur Zahlen mit oder ohne Nachkommastellen eingeben!");
    }
    else
    {
        goto beginOperationValidation;
    }
    Console.WriteLine(validator.GetUserInputValidationReport(userEquation, ref equationPassed));
    new Operator(userdataLocation, userName).SetCurrentUserScoreByUserName(userName, bonus, malus, equationPassed);
    goto beginOperationValidation;
}

static string GetPlaintextPasswordByMaskedInput()
{
    var password = new StringBuilder();
    ConsoleKeyInfo key;
    while (true)
    {
        key = Console.ReadKey(true);
        if (key.Key is ConsoleKey.Enter)
            break;
        password.Append(key.KeyChar);
        Console.Write("*");
    }

    return password.ToString();
}
