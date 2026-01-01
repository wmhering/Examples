using System.Text.RegularExpressions;

using FluentResults;

using ExpressionEvaluatorExample.ScriptSupport;
using System.Windows.Markup;
using Microsoft.CodeAnalysis.Scripting;

namespace ExpressionEvaluatorExample;

public class RuleContext : ScriptContext
{
    public RuleContext(IEnumerable<Value> values, Script script) : base(values)
    {
        Script = script ?? throw new ArgumentNullException(nameof(script));
    }

    public void SetValue(string name, PrimativeType type, string value)
    {
        if (_values.ContainsKey(name) && _values[name].Type != type)
            throw new InvalidCastException();
        _values[name] = new Value(name, type, value);
    }

    public string GetTextForValue(string name)
    {
        if (_values.ContainsKey(name))
            return _values[name].Text ?? "";
        return "";
    }

    public Script Script { get; }
}
