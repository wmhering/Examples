using System.Text.Json;
using System.Text.Json.Nodes;

namespace JsonExamples;

internal class Program
{
    static void Main(string[] args)
    {
        var program = new Program();
        program.EditJsonInCode();
    }

    void EditJsonInCode()
    {
        // This methods adds an "qty" value to all lines of an invoice that don't already have one.
        var orgInvoice = "{\"name\":\"Customer Name\",\"lines\":[{\"desc\":\"Item 1\",\"price\":15},{\"desc\":\"Item 2\",\"qty\":3,\"price\":10}]}";
        Console.WriteLine($"Original JSON: {orgInvoice}");
        var rootNode = JsonNode.Parse(orgInvoice);
        if (!(rootNode is JsonObject) || !rootNode.AsObject().ContainsKey("lines") || !(rootNode.AsObject()["lines"] is JsonArray))
        {
            Console.WriteLine("Root node must be an object with a value named lines of type array");
            return;
        }
        var linesArray = rootNode.AsObject()["lines"].AsArray();
        foreach(var lineNode in linesArray)
        {
            if (!(lineNode is JsonObject))
            {
                Console.WriteLine("Line node must be and object");
                return;
            }
            var lineObject = (JsonObject)lineNode;
            if (!lineObject.ContainsKey("qty"))
                lineObject.Add("qty", JsonValue.Create(1));
            else if (!(lineObject["qty"] is JsonValue) || lineObject["qty"].AsValue().GetValueKind() != JsonValueKind.Number)
            {
                Console.WriteLine("Qty node of a line node must be number value.");
                return;
            }
        }
        var updInvoice = rootNode.ToJsonString();
        Console.WriteLine($"Updated JSON: {updInvoice}");
        
    }
}
