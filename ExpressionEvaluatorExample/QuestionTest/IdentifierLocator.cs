using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExpressionEvaluatorExample.QuestionTest;

internal class IdentifierLocator
{
    public List<string> FindIdentifiers(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var root = syntaxTree.GetRoot();

        var identifiers = new List<string>();
        CollectIdentifiers(root, identifiers);
        return identifiers;
    }

    private static void CollectIdentifiers(SyntaxNode node, List<string> identifiers)
    {
        switch (node)
        {
            case InvocationExpressionSyntax invocation:
                var target = invocation.Expression is MemberAccessExpressionSyntax memberAccess
                    ? memberAccess.Expression
                    : invocation.Expression;
                CollectIdentifiers(target, identifiers);
                foreach (var argument in invocation.ArgumentList.Arguments)
                {
                    CollectIdentifiers(argument.Expression, identifiers);
                }
                return;

            case MemberAccessExpressionSyntax { Name.Identifier.ValueText: "Value" } valueAccess:
                CollectIdentifiers(valueAccess.Expression, identifiers);
                return;

            case IdentifierNameSyntax or MemberAccessExpressionSyntax or ElementAccessExpressionSyntax:
                identifiers.Add(node.ToString());
                return;

            default:
                foreach (var child in node.ChildNodes())
                {
                    CollectIdentifiers(child, identifiers);
                }
                return;
        }
    }
}
