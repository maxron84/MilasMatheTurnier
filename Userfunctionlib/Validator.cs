using System.Text;

namespace Userfunctionlib;

public class Validator
{
    private Random _random;
    private List<Func<double, double, double>> _operations;
    private List<double> _numbers;
    private Dictionary<int, (List<double>? modulosDiv, int maxValueAddSub, int maxValueMul, int maxValueDiv, int maxNumbersAddSub, int maxNumbersMulDiv, bool canNextGreaterThanPreviousAddSubMul, bool canNextGreaterThanPreviousDiv, bool isMaxHalfOfPreviousDiv)> _constraintsLookup = new()
    {
        { 6, (null, 21, -1, -1, 2, -1, false, false, true) },
        { 7, (null, 101, 11, -1, 3, 2, false, false, true) },
        { 8, (new List<double>() { 2 }, 1_001, 21, 101, 3, 2, false, false, true) },
        { 9, (new List<double>() { 2, 3, 5 }, 100_001, 101, 1_001, 4, 2, true, false, false) }
    };
    private int _targetUserAge;
    private StringBuilder _stringBuilder = new();
    private Calculator<double> _calculator = new();

    public Validator(int targetUserAge)
    {
        _targetUserAge = targetUserAge < 9 ? targetUserAge : 9;
        _random = new();
        _operations = new();
        _numbers = new();
    }

    public string GetUsertaskReport(string userOperator)
    {
        _stringBuilder.Clear();
        _ = CreateUsertaskArithmetic(userOperator);
        for (int i = 0; i < _numbers.Count() + 1; i++)
        {
            if (i > 0 && i < _numbers.Count())
                _stringBuilder.Append(userOperator + " ");
            if (i == _numbers.Count())
                _stringBuilder.Append("= ");
            if (i < _numbers.Count())
                _stringBuilder.Append(_numbers[i].ToString() + " ");
        }
        return _stringBuilder.ToString();
    }

    public string GetUserInputValidationReport(double userEquation, ref bool equationPassed)
    {
        double result = _calculator.ComputeOperationSequence_Dumb(_operations, _numbers);
        if (userEquation != result)
        {
            equationPassed = false;
            return $"Dein Ergebnis ist leider falsch! :( " + "Richtig wäre: {result}";
        }
        equationPassed = true;
        return "Dein Ergebnis ist richtig! :)";
    }

    private Task CreateUsertaskArithmetic(string userOperator)
    {
        var modulosDiv = _constraintsLookup[_targetUserAge].modulosDiv ?? new List<double> { 1 };
        var quality = 0;
        var quantity = 0;
        var canNextGreaterPrev = userOperator == "/" ? _constraintsLookup[_targetUserAge].canNextGreaterThanPreviousDiv : _constraintsLookup[_targetUserAge].canNextGreaterThanPreviousAddSubMul;
        var isMaxHalfDiv = _constraintsLookup[_targetUserAge].isMaxHalfOfPreviousDiv;

        if (userOperator == "+" || userOperator == "-")
        {
            quality = _constraintsLookup[_targetUserAge].maxValueAddSub;
            quantity = _constraintsLookup[_targetUserAge].maxNumbersAddSub > 2 ? _random.Next(2, _constraintsLookup[_targetUserAge].maxNumbersAddSub + 1) : 2;
            _ = AddNumbersByConstraints_Bruteforce(userOperator, quantity, quality, modulosDiv, canNextGreaterPrev, isMaxHalfDiv);
        }

        if (userOperator == "*" || userOperator == "/")
        {
            quality = userOperator == "/" ? _constraintsLookup[_targetUserAge].maxValueDiv : _constraintsLookup[_targetUserAge].maxValueMul;
            quantity = _constraintsLookup[_targetUserAge].maxNumbersMulDiv > 2 ? _random.Next(2, _constraintsLookup[_targetUserAge].maxNumbersMulDiv + 1) : 2;
            _ = AddNumbersByConstraints_Bruteforce(userOperator, quantity, quality, modulosDiv, canNextGreaterPrev, isMaxHalfDiv);
        }

        for (int i = 0; i < _numbers.Count() - 1; i++)
        {
            var target = userOperator == "+" ? _calculator.FuncAdd : userOperator == "-" ? _calculator.FuncSub : userOperator == "*" ? _calculator.FuncMul : _calculator.FuncDiv;
            _operations.Add(target);
        }

        return Task.CompletedTask;
    }

    private Task AddNumbersByConstraints_Bruteforce(string userOperator, int quantity, int maxValue, List<double> modulosDiv, bool canNextGreaterPrev, bool isMaxHalfDiv)
    {
        for (int i = 0; i < quantity; i++)
        {
            double candidate = (double)_random.Next(1, maxValue);
            if ((!canNextGreaterPrev && i > 0 && candidate > _numbers[i - 1])
                | (userOperator == "/" && !(modulosDiv.Any(modulo => candidate % modulo == 0)))
                | (isMaxHalfDiv && i > 0 && candidate > _numbers[i - 1] / 2))
            {
                i--;
                continue;
            }
            _numbers.Add(candidate);
        }

        return Task.CompletedTask;
    }
}
