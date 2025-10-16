namespace ExpressionEvaluatorExample;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter \"quit\" to quit");
        Console.WriteLine("Enter \"roslyn\" or \"expresso\" to change evaluator");
        Console.WriteLine("Enter \"clear\" to clear saved variables");
        Console.WriteLine("Or an assignment expression that may use any variables previously created");
        var values = new Dictionary<string, object>();
        while (true)
        {
            var line = Console.ReadLine();
            line = line ?? "quit";
            if (line.Trim().ToLower() == "quit")
                break;
            if (line.Trim().ToLower() == "roslyn")
            {
                Evaluator = new RoslynEvaluator();
                Console.WriteLine("Using Roslyn expression evaluator");
                continue;
            }
            if (line.Trim().ToLower() == "expresso")
            {
                Evaluator = new ExpressoEvaluator();
                Console.WriteLine("Using DynamicExpresso expression evaluator");
                continue;
            }
            if (line.Trim().ToLower() == "clear")
            {
                values.Clear();
                Console.WriteLine("All variables have been cleared");
                continue;
            }
            if (line.Trim().Length == 0)
                continue;
            var result = Evaluator.Evaluate(line, values);
            if (result.IsFailed)
                foreach (var error in result.Errors)
                    Console.WriteLine(error);
            else
                values[result.Value.Variable] = result.Value.Value;
        }
        DisplayGlobals(values);
        
        Console.Write("Press enter to continue...");
        Console.ReadLine();
    }

    static EvaluatorBase Evaluator { get; set; } = new RoslynEvaluator();

    static void DisplayGlobals(Dictionary<string, object> values)
    {
        if (values.Count == 0)
            return;
        Console.WriteLine();
        Console.WriteLine("Globals:");
        foreach (var key in values.Keys.OrderBy(s => s, StringComparer.InvariantCultureIgnoreCase))
            Console.WriteLine($"  {values[key].GetType().Name} {key} = {values[key]}");
        Console.WriteLine();
    }
}
