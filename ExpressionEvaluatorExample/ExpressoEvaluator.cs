using DynamicExpresso;
using FluentResults;

namespace ExpressionEvaluatorExample
{
    public class ExpressoEvaluator : EvaluatorBase
    {
        public override Result<EvaluationResult> Evaluate(string line, IEnumerable<KeyValuePair<string, object>> variables)
        {
            var getStatementPartsResult = GetStatementParts(line);
            if (getStatementPartsResult.IsFailed)
                return (Result<EvaluationResult>)getStatementPartsResult.Errors;
            var statementParts = getStatementPartsResult.Value;

            try
            {
                var parameters = variables
                    .Select(v => new Parameter(v.Key, v.Value))
                    .ToArray();
                var interpreter = new Interpreter();
                var value = interpreter.Eval(statementParts.Expression, parameters);
                return new EvaluationResult(statementParts.Variable, value);
            }
            catch (Exception ex)
            {
                return new Error($"Unexpected {ex.GetType().Name} while evaluating '{statementParts.Expression}': {ex.Message}");
            }
        }
    }
}
