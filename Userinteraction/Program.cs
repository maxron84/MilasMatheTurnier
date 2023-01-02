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
var @operator = new Operator(userdataLocation, string.Empty);

beginIntroduction:
Console.WriteLine("# Gebe eine der folgenden Ziffern ein:\n\n# 1: Neues Spiel beginnen\n# 2: Bestenliste anzeigen\n# 3: Konsole aufräumen\n");
userInput = Console.ReadLine();
// Delete everything from Json-File
if (userInput == "i am aware of all my data being deleted")
{
    Console.WriteLine("# DEBUG: ARE YOU SURE ???");
    userInput = Console.ReadLine();
    if (userInput == "YES")
    {
        Console.WriteLine("# DEBUG: BEGIN DELETING DATA FROM JSON FILE:");
        var deleteDataTask = Task.Run(async () => await @operator.DeleteAllUsersAsync());
        ReportTaskProgressToConsole(deleteDataTask, "DEBUG: DELETING DATA FROM JSON FILE", "DEBUG: ALL DATA SUCCESSFULLY DELETED FROM JSON FILE!");
        // Write many objects to Json-File to test the application with big data
        Console.WriteLine("# DEBUG: WRITE VERY LARGE SET OF OBJECTS TO THE JSON-FILE FOR TESTING PURPOSES ???");
        userInput = Console.ReadLine();
        if (userInput == "YES")
        {
            Console.WriteLine("# DEBUG: BEGIN WRITING TO JSON FILE:");
            var createDataTask = Task.Run(async () => await @operator.CreateExampleWithBigData());
            ReportTaskProgressToConsole(createDataTask, "DEBUG: WRITING DATA TO JSON FILE", "DEBUG: ALL DATA SUCCESSFULLY WRITTEN TO JSON FILE!");
        }
    }
    goto beginIntroduction;
}
if (userInput != "1" && userInput != "2" && userInput != "3")
    goto beginIntroduction;
if (userInput == "2")
{
    var sortedUsersTask = Task.Run(async () => await @operator.GetAllUsersSortedByUserScoreDescAsync());
    ReportTaskProgressToConsole(sortedUsersTask, "Daten werden geladen", string.Empty);
    Console.WriteLine(sortedUsersTask.Result);
    await Task.Delay(500);
    await foreach (var result in @operator.GetEachUserdataModelReportAsync())
        Console.WriteLine(result);
    Console.WriteLine();
    goto beginIntroduction;
}
else if (userInput == "3")
{
    Console.Clear();
    goto beginIntroduction;
}
else
{
    Console.WriteLine("# Wie heißt du?");
}
while (true)
{
    userName = Console.ReadLine();
    if (!string.IsNullOrEmpty(userName) && !string.IsNullOrWhiteSpace(userName) && userName.Length >= userNameSize.Min && userName.Length <= userNameSize.Max)
        break;
    Console.WriteLine($"# Bitte einen Namen mit einer Länge von {userNameSize.Min} bis {userNameSize.Max} Zeichen eingeben!");
}

beginAgeValidation:
Console.WriteLine("# Wie alt bist du?");
userInput = Console.ReadLine();
if (!Int32.TryParse(userInput, out userAge))
{
    Console.WriteLine("# Bitte nur Zahlen eingeben!");
    goto beginAgeValidation;
}
if (userAge < 6)
{
    Console.WriteLine("# Du musst mindestens 6 Jahre alt sein!");
    goto beginAgeValidation;
}

bonus = userSetupLookup.ContainsKey(userAge) ? userSetupLookup[userAge].bonus : bonusOpenEnd;
malus = userSetupLookup.ContainsKey(userAge) ? userSetupLookup[userAge].malus : malusOpenEnd;
@operator = new Operator(userdataLocation, userName);

if (@operator.IsUserAlreadyExistingAsync(userName).Result)
{
    var targetPassword = @operator.GetUserPasswordAsync(userName).Result;
    if (!string.IsNullOrEmpty(targetPassword))
    {
    beginPasswordInputPrompt:
        Console.WriteLine("# Bitte gebe dein Passwort ein:");
        if (GetPlaintextPasswordByMaskedInput() != targetPassword)
        {
            Console.WriteLine("\n# Das eingegebene Passwort ist falsch!");
            goto beginPasswordInputPrompt;
        }
    }
    _ = @operator.UpdateUserAgeByUserName(userName, userAge);
    userScore = @operator.GetUserScoreByUserNameAsync(userName).Result;
    Console.WriteLine($"\n# Willkommen zurück, {userName}! Dein aktueller Punktestand lautet: {userScore}");
}
else
{
    var userPassword = string.Empty;
    while (true)
    {
        Console.WriteLine("# Wähle ein Passwort aus mindestens 3 Zeichen oder drücke einfach Enter wenn du kein Passwort setzen möchtest:");
        userInput = GetPlaintextPasswordByMaskedInput();
        if (userInput.Length > 2)
            break;
        Console.WriteLine("\n# Dein Passwort muss aus mindestens 3 Zeichen bestehen!");
    }
    if (!string.IsNullOrEmpty(userInput))
        userPassword = userInput;
    _ = @operator.CreateNewUserAsync(userName, userAge, userPassword);
    Console.WriteLine($"\n# Willkommen, {userName}! Du bist also {userAge} Jahre alt und beginnst daher mit dem Schwierigkeitsgrad {malus}. Viel Spaß!\n");
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
            Console.WriteLine("# Bitte nur Zahlen mit höchstens 2 gerundeten Nachkommastellen eingeben!");
    }
    else
    {
        goto beginOperationValidation;
    }
    Console.WriteLine(validator.GetUserInputValidationReport(Math.Round(userEquation, 2), ref equationPassed));
    _ = @operator.SetCurrentUserScoreByUserNameAsync(userName, bonus, malus, equationPassed);
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
        if (key.KeyChar > 32 && key.KeyChar < 127)
        {
            password.Append(key.KeyChar);
            Console.Write("*");
        }
        else if (key.Key is ConsoleKey.Backspace && password.Length > 0)
        {
            password.Remove(password.Length - 1, 1);
            Console.Write("\b \b");
        }
    }

    return password.ToString();
}

static void RewriteLastLine(string replacement)
{
    int originalRow = Console.CursorTop;
    int originalCol = Console.CursorLeft;

    Console.SetCursorPosition(0, originalRow);
    Console.Write(new string(' ', Console.BufferWidth));
    Console.SetCursorPosition(0, originalRow);
    Console.Write(replacement);
    Console.SetCursorPosition(originalCol, originalRow);
}

static void ReportTaskProgressToConsole(Task taskToBeReported, string reportingText, string onIsCompletedText)
{
    while (!taskToBeReported.IsCompleted)
    {
        RewriteLastLine("# " + reportingText + ".");
        Thread.Sleep(500);
        RewriteLastLine("# " + reportingText + "..");
        Thread.Sleep(500);
        RewriteLastLine("# " + reportingText + "...");
        Thread.Sleep(500);
    }
    Console.WriteLine("# " + onIsCompletedText);
}
