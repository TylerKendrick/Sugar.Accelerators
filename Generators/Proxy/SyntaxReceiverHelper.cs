using Microsoft.CodeAnalysis;

namespace Sugar.Accelerators.Generators.Proxy;
using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public partial class ProxyGenerator
{
    private static class SyntaxReceiverHelper
    {
        public static T GetNode<T>(CSharpSyntaxNode node)
            where T : CSharpSyntaxNode
        {
            SyntaxNode? targetNode = node.Parent;
            while (targetNode != null)
            {
                if (targetNode is T syntax) return syntax;
                targetNode = targetNode.Parent;
            }
            throw new Exception($"Couldn't find a {typeof(T)} node in the parent heirarchy.");
        }

        public static ArgumentSyntax Convert(ParameterSyntax parameterSyntax)
        {
            var expression = SyntaxFactory.ParseExpression(parameterSyntax.Identifier.ToFullString());
            return SyntaxFactory.Argument(expression);
        }
        public static ArgumentListSyntax Convert(ParameterListSyntax parameterListSyntax)
        {
            var argumentList = parameterListSyntax.Parameters.Select(Convert);
            var separatedList = SyntaxFactory.SeparatedList<ArgumentSyntax>(argumentList);
            return SyntaxFactory.ArgumentList(separatedList);
        }

        public static MethodDeclarationSyntax CreateProxy(SyntaxToken proxyToken, MethodDeclarationSyntax other)
        {
            var proxyName = SyntaxFactory.IdentifierName(proxyToken);
            var methodArgs = Convert(other.ParameterList);
            var methodName = SyntaxFactory.IdentifierName(other.Identifier);
            var memberAccess = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                proxyName, methodName)
                .WithOperatorToken(SyntaxFactory.Token(SyntaxKind.DotToken));
            var methodInvocation = SyntaxFactory.InvocationExpression(memberAccess, methodArgs);
            var expression = SyntaxFactory.ArrowExpressionClause(methodInvocation);
            return other.WithExpressionBody(expression);
        }
    }
}