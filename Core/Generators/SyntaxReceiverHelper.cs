/// <summary>
/// Helper class for working with <see cref="CSharpSyntaxNode"/> instances.
/// </summary>
public static class SyntaxReceiverHelper
{
    /// <summary>
    /// Gets the first <typeparamref name="T"/> node found in the parent hierarchy of the specified <paramref name="node"/>.
    /// </summary>
    /// <typeparam name="T">The type of node to retrieve.</typeparam>
    /// <param name="node">The node to search from.</param>
    /// <returns>The first <typeparamref name="T"/> node found in the parent hierarchy of the specified <paramref name="node"/>.</returns>
    /// <exception cref="Exception">Thrown when no <typeparamref name="T"/> node is found in the parent hierarchy.</exception>
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

    /// <summary>
    /// Converts the specified <see cref="ParameterSyntax"/> instance to an <see cref="ArgumentSyntax"/> instance.
    /// </summary>
    /// <param name="parameterSyntax">The <see cref="ParameterSyntax"/> instance to convert.</param>
    /// <returns>The converted <see cref="ArgumentSyntax"/> instance.</returns>
    public static ArgumentSyntax Convert(ParameterSyntax parameterSyntax)
    {
        var expression = SyntaxFactory.ParseExpression(parameterSyntax.Identifier.ToFullString());
        return SyntaxFactory.Argument(expression);
    }
    /// <summary>
    /// Converts the specified <see cref="ParameterListSyntax"/> instance to an <see cref="ArgumentListSyntax"/> instance.
    /// </summary>
    /// <param name="parameterListSyntax">The <see cref="ParameterListSyntax"/> instance to convert.</param>
    /// <returns>The converted <see cref="ArgumentListSyntax"/> instance.</returns>
    public static ArgumentListSyntax Convert(ParameterListSyntax parameterListSyntax)
    {
        var argumentList = parameterListSyntax.Parameters.Select(Convert);
        var separatedList = SyntaxFactory.SeparatedList<ArgumentSyntax>(argumentList);
        return SyntaxFactory.ArgumentList(separatedList);
    }

    /// <summary>
    /// Creates a proxy method declaration based on the specified <see cref="SyntaxToken"/> and <see cref="MethodDeclarationSyntax"/> instances.
    /// </summary>
    /// <param name="proxyToken">The <see cref="SyntaxToken"/> to use as the name of the proxy method.</param>
    /// <param name="other">The original <see cref="MethodDeclarationSyntax"/> instance to create the proxy method from.</param>
    /// <returns>The newly created proxy <see cref="MethodDeclarationSyntax"/> instance.</returns>
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