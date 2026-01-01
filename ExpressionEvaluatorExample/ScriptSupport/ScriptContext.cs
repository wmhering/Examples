namespace ExpressionEvaluatorExample.ScriptSupport
{
    public class ScriptContext
    {
        protected readonly Dictionary<string, Value> _values;

        internal ScriptContext(IEnumerable<Value> values)
        {
            _values = values?.ToDictionary(v => v.Name, StringComparer.InvariantCultureIgnoreCase) ?? throw new ArgumentNullException(nameof(values));
        }

        public bool? GetLogicalValue(string name) =>
            bool.TryParse(GetTextForValueWithTypeCheck(name, PrimativeType.Logical), out var value) ? value : null;

        public decimal? GetNumericValue(string name) =>
            decimal.TryParse(GetTextForValueWithTypeCheck(name, PrimativeType.Numeric), out var value) ? value : null;

        public DateTime? GetTemporalValue(string name) =>
            DateTime.TryParse(GetTextForValueWithTypeCheck(name, PrimativeType.Temporal), out var value) ? value : null;

        public string? GetTextValue(string name) =>
            GetTextForValueWithTypeCheck(name, PrimativeType.Text);

        private string GetTextForValueWithTypeCheck(string name, PrimativeType type)
        {
            if (!_values.ContainsKey(name))
                return "";
            var result = _values[name];
            if (result.Type == type)
                return result.Text;
            throw new InvalidCastException($"Expected {type} but value is {result.Type}");
        }
    }
}
