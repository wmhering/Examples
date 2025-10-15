using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;
using System.ComponentModel;
using System.Dynamic;
using System.Text.RegularExpressions;

namespace RoslynExample;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter quit to quit");
        Console.WriteLine("Or an assignment expression that may use any variables previously created");
        var values = new Dictionary<string, object>();
        while (true)
        {
            var line = Console.ReadLine();
            line = line ?? "quit";
            if (line.Trim().ToLower() == "quit")
                break;
            if (line.Trim().Length == 0)
                continue;
            ProcessLine(line, values);
        }
        DisplayGlobals(values);
        Console.Write("Press enter to continue...");
        Console.ReadLine();
    }
    
    static void ProcessLine(string line, Dictionary<string, object> values)
    {
        string left, right;
        bool ok;
        object value;

        (left, right, ok) = SplitLine(line);
        if (!ok) return;
        var globals = CreateGlobals(values);
        (value, ok) = EvaluateExpression(right, globals);
        if (!ok) return;
        values[left] = value;
    }

    static (string, string, bool) SplitLine(string line)
    {
        string left = "", right = "";
        var i = line.IndexOf('=');
        if (i >= 1 && i < line.Length - 1)
        {
            left = line.Substring(0, i).Trim();
            right=line.Substring(i + 1).Trim();
        }
        var ok = true;
        if (left.Length == 0 || right.Length == 0)
        {
            ok = false;
            Console.WriteLine("You must enter an assignment statement");
        }
        else if(!Regex.IsMatch(left, "^[a-zA-Z][a-zA-Z0-9_]*$"))
        {
            ok = false;
            Console.WriteLine("You must give a valid identifier name");
        }
        return (left, right, ok);
    }

    //static object? CreateGlobals(Dictionary<string, object> values)
    //{
    //    if (values.Count == 0)
    //        return null;
    //    var result = new ExpandoObject();
    //    foreach (var key in values.Keys)
    //        (result as IDictionary<string, object>)[key] = values[key];
    //    return result;
    //}

    static object? CreateGlobals(Dictionary<string, object> values)
    {
        if (values.Count == 0)
            return null;
        string code = @"
public class Globals {
    public int A => 30;
}

return new Globals();
";
        var result = CSharpScript.EvaluateAsync(code).Result;
        return result;
    }

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

    static (object, bool) EvaluateExpression(string text, object globals)
    {
        try
        {
            var options = ScriptOptions.Default;
            var result = CSharpScript.EvaluateAsync(text, options, globals, globals?.GetType()).Result;
            return (result, true);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return (false, false);
        }
    }
}
