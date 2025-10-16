using System.Text.RegularExpressions;

using FluentResults;

namespace ExpressionEvaluatorExample;

public abstract class EvaluatorBase
{
    public abstract Result<EvaluationResult> Evaluate(string line, IEnumerable<KeyValuePair<string, object>> variables);
    public record EvaluationResult(string Variable, object Value);

    protected Result<StatementParts> GetStatementParts(string line)
    {
        string variable = "", expression = "";
        var i = line.IndexOf('=');
        if (i >= 1 && i < line.Length - 1)
        {
            variable = line.Substring(0, i).Trim();
            expression = line.Substring(i + 1).Trim();
        }

        if (variable.Length == 0 || expression.Length == 0)
            return new Error("You must enter an assignment statement");

        if (!Regex.IsMatch(variable, "^[a-zA-Z][a-zA-Z0-9_]*$"))
            return new Error("You must give a valid identifier name");

        return new StatementParts(variable, expression);

    }
    protected record StatementParts(string Variable, string Expression);
}
