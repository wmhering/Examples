using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;

using ExpressionEvaluatorExample.ScriptSupport;

namespace ExpressionEvaluatorExample;

public class Program
{
    static void Main(string[] args)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText("return TemporalValue.AddDays(Files[0].NumericValue > 29 ? 1 : NumericValue);");
        SyntaxNode rootNode = syntaxTree.GetRoot();

        var context = InitializeContext();

        EvaluateCode(context, "return DateTime.Now.Second;", "NumericValue", PrimativeType.Numeric);
        EvaluateCode(context, "if (NumericValue > 20)\r\n  return DateTime.Now;\r\nreturn DateTime.Today;", "TemporalValue", PrimativeType.Temporal);
        EvaluateCode(context, "return NumericValue > 29;", "LogicalValue", PrimativeType.Logical);
        EvaluateCode(context, "return NumericValue.ToString().Substring(0,1);", "TextValue", PrimativeType.Text);
        EvaluateCode(context, "throw new NotSupportedException(\"This will not compile\")", "TextValue", PrimativeType.Text);
        EvaluateCode(context, "throw new NotSupportedException(\"Exception handling test\");", "TextValue", PrimativeType.Text);

        Console.WriteLine();
        Console.Write("Press [Enter] to finish...");
        Console.ReadLine();
    }

    static RuleContext InitializeContext()
    {
        var code = @"#nullable enable
bool?     LogicalValue  { get { return GetLogicalValue(""LogicalValue""); } }
decimal?  NumericValue  { get { return GetNumericValue(""NumericValue""); } }
DateTime? TemporalValue { get { return GetTemporalValue(""TemporalValue""); } }
string?   TextValue     { get { return GetTextValue(""TextValue""); } }
";
        var options = ScriptOptions.Default
            .AddReferences("System")
            .AddReferences(typeof(Program).Assembly)
            .WithImports("System", "ExpressionEvaluatorExample.ScriptSupport")
        ;
        var start = DateTime.Now;
        var script = CSharpScript.Create<object>(code, options, globalsType: typeof(ScriptContext));
        var compileResult = script.Compile();
        Console.WriteLine($"Compiled context in {(DateTime.Now - start).TotalSeconds}s");

        var result = new RuleContext([], script);
        DisplayValues(result);
        return result;
    }

    static void EvaluateCode(RuleContext context, string code, string assignTo, PrimativeType type)
    {
        Console.WriteLine();
        Console.WriteLine($"Assigning {assignTo} the result of the following code:");
        Console.ForegroundColor = ConsoleColor.Cyan; Console.WriteLine(code); Console.ResetColor();
        try
        {
            var start = DateTime.Now;
            var result = context.Script.ContinueWith(code).RunAsync((ScriptContext)context, catchException: ex => true).Result;
            if (result.Exception != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"The code threw a(n) {result.Exception.GetType().Name}: {result.Exception.Message}");
                Console.ResetColor();
                return;
            }
            context.SetValue(assignTo, type, result.ReturnValue?.ToString() ?? "");
            Console.WriteLine($"Evaluated code in {(DateTime.Now - start).TotalSeconds}s");
            DisplayValues(context);
        }
        catch (CompilationErrorException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("The following error(s) occured while compiling code:");
            foreach (var diagnostic in ex.Diagnostics)
                Console.WriteLine($"  {diagnostic}");
            Console.ResetColor();
        }
    }

    static void DisplayValues(RuleContext context)
    {
        Console.WriteLine("Context values:");
        Console.WriteLine($"  LogicalValue:  \"{context.GetTextForValue("LogicalValue")}\"");
        Console.WriteLine($"  NumericValue:  \"{context.GetTextForValue("NumericValue")}\"");
        Console.WriteLine($"  TemporalValue: \"{context.GetTextForValue("TemporalValue")}\"");
        Console.WriteLine($"  TextValue:     \"{context.GetTextForValue("TextValue")}\"");
    }

    static void DisplayAST(SyntaxNode node, int level = 0)
    {
        Console.Write(new string(' ', level * 4));
        Console.Write(node.GetType().Name);
        Console.Write(" - ");
        Console.WriteLine(node.GetText());
        foreach (var child in node.ChildNodes())
            DisplayAST(child, level + 1);
    }

    static IEnumerable<string> GetIdentifiers(SyntaxNode node)
    {
        if (node is IdentifierNameSyntax identifier && IdentifierFilter((IdentifierNameSyntax)node))            
            yield return identifier.Identifier.Text;
        foreach (var child in node.ChildNodes())
            foreach (var id in GetIdentifiers(child))
                yield return id;
    }

    static bool IdentifierFilter(IdentifierNameSyntax identifier)
    {
        // Skip method names
        if (identifier.Parent is MemberAccessExpressionSyntax && identifier == identifier.Parent.ChildNodes().Skip(1).First())
            return false;
        return true;
    }
}
