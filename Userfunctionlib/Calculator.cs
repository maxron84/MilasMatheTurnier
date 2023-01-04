namespace Userfunctionlib;

public class Calculator<T> where T : INumber<T>
{
    public Func<T, T, T> FuncAdd { get => (x, y) => x + y; }
    public Func<T, T, T> FuncSub { get => (x, y) => x - y; }
    public Func<T, T, T> FuncMul { get => (x, y) => x * y; }
    public Func<T, T, T> FuncDiv { get => (x, y) => x / y; }

    /// <summary>
    /// Keine Punkt-Vor-Strich Berücksichtigung, dafür werden auch andere Operationen als nur + - * / akzeptiert!
    /// 
    /// Diese Methode in C# nimmt eine Liste von Funktionen und eine Liste von Zahlen als Eingabeparameter und führt die Funktionen auf den Zahlen aus.
    /// Die Funktionen sind in der Form "Func<T, T, T>" definiert, was bedeutet, dass sie zwei Eingabeparameter vom Typ T haben und einen Rückgabewert vom Typ T haben.
    /// Die Methode iteriert über die Liste der Funktionen und führt jede Funktion auf die Zahlen aus.
    /// Der erste Aufruf der Funktion verwendet das erste Element der Zahlenliste als ersten Eingabeparameter und das zweite Element der Zahlenliste als zweiten Eingabeparameter.
    /// Der Rückgabewert der Funktion wird dann in "result" gespeichert.
    /// Die nächste Iteration verwendet den Wert von "result" als ersten Eingabeparameter und das dritte Element der Zahlenliste als zweiten Eingabeparameter.
    /// Dies wird fortgesetzt, bis alle Funktionen auf alle Zahlen angewendet wurden.
    /// </summary>
    /// <param name="operations"></param>
    /// <param name="numbers"></param>
    /// <returns>Das Ergebnis der Berechnung.</returns>
    public T ComputeOperationSequence_Dumb(IList<Func<T, T, T>> operations, IList<T> numbers)
    {
        T? result = numbers[0];

        for (int i = 0; i < operations.Count(); i++)
            result = operations[i](result, numbers[i + 1]);

        return result;
    }

    /// <summary>
    /// Punkt-Vor-Strich wird berücksichtigt, jedoch nur + - * / wird akzeptiert!
    /// 
    /// Diese Version der Methode verwendet einen Stack, um die Operanden und Ergebnisse der Operationen zu speichern.
    /// Wenn eine Multiplikation oder Division gefunden wird, werden die beiden letzten Elemente vom Stack entfernt und die Operation wird ausgeführt.
    /// Das Ergebnis wird dann wieder auf den Stack gelegt. Wenn hingegen eine Addition oder Subtraktion gefunden wird,
    /// wird der nächste Operand auf den Stack gelegt und dann werden die beiden letzten Elemente vom Stack entfernt und die Operation wird ausgeführt.
    /// Das Ergebnis wird wieder auf den Stack gelegt. Am Ende des Durchlaufs enthält der Stack das endgültige Ergebnis.
    /// </summary>
    /// <param name="operations"></param>
    /// <param name="numbers"></param>
    /// <returns>Das Ergebnis der Berechnung.</returns>
    /// <exception cref="ArgumentException">Wenn einer der Operationen nicht + - * / ist.</exception>
    public T ComputeOperationSequence_Smart(IList<Func<T, T, T>> operations, IList<T> numbers)
    {
        T? result = numbers[0];
        Stack<T> stack = new Stack<T>();
        stack.Push(result);

        for (int i = 0; i < operations.Count(); i++)
        {
            if (operations[i].Method.Name == nameof(FuncMul) || operations[i].Method.Name == nameof(FuncDiv))
            {
                T rhs = stack.Pop();
                T lhs = stack.Pop();
                result = operations[i](lhs, rhs);
                stack.Push(result);
            }
            else if (operations[i].Method.Name == nameof(FuncAdd) || operations[i].Method.Name == nameof(FuncSub))
            {
                stack.Push(numbers[i + 1]);
                T rhs = stack.Pop();
                T lhs = stack.Pop();
                result = operations[i](lhs, rhs);
                stack.Push(result);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        return result;
    }
}
