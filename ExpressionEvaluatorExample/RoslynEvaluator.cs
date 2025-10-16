using System.Text;

using FluentResults;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace ExpressionEvaluatorExample
{
    public class RoslynEvaluator : EvaluatorBase
    {
        public override Result<EvaluationResult> Evaluate(string line, IEnumerable<KeyValuePair<string, object>> variables)
        {
            var getStatementPartsResult = GetStatementParts(line);
            if (getStatementPartsResult.IsFailed)
                return (Result<EvaluationResult>)getStatementPartsResult.Errors;
            var statementParts = getStatementPartsResult.Value;

            try
            {
                var code = ScriptGlobals(variables) + "return " + statementParts.Expression + ";";
                var result = CSharpScript.EvaluateAsync(code).Result;
                return new EvaluationResult(statementParts.Variable, result);
            }
            catch (Exception ex)
            {
                return new Error($"Unexpected {ex.GetType().Name} while evaluating '{statementParts.Expression}': {ex.Message}");
            }
        }

        private string ScriptGlobals(IEnumerable<KeyValuePair<string, object>> variables)
        {
            var result = new StringBuilder();
            foreach (var variable in variables)
                result.AppendLine($"var {variable.Key} = {ConstantText(variable.Value)};");
            return result.ToString();
        }

        private string ConstantText(object value)
        {
            var type = value.GetType();
            if (type==typeof(string))
                return $"\"{value.ToString()}\"";
            if (type == typeof(int))
                return value.ToString();

            throw new NotImplementedException($"Cannot convert {value.GetType().Name}");
        }
    }
}
