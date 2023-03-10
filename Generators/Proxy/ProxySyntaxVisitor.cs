namespace Sugar.Accelerators.Generators;

public class ProxySyntaxVisitor : CSharpSyntaxVisitor<ClassDeclarationSyntax>
{
    private readonly SyntaxToken _proxyToken;
    private readonly TypeSyntax _proxyType;

    public ProxySyntaxVisitor(TypeSyntax proxyType)
    {
        _proxyType = proxyType;
        _proxyToken = SyntaxFactory.Identifier("_proxyHandle");
    }

    public override ClassDeclarationSyntax VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var modifiers = new[]{SyntaxKind.PrivateKeyword, SyntaxKind.ReadOnlyKeyword};
        var tokens = modifiers.Select(SyntaxFactory.Token).ToArray();
        var declaration = SyntaxFactory.VariableDeclaration(_proxyType)
            .AddVariables(SyntaxFactory.VariableDeclarator(_proxyToken));
        var field = SyntaxFactory.FieldDeclaration(declaration)
            .AddModifiers(tokens);

        node = node.AddMembers(field);
        node = node.AddBaseListTypes(SyntaxFactory.SimpleBaseType(_proxyType));
        var paramName = SyntaxFactory.IdentifierName("proxyHandle");
        var proxyName = SyntaxFactory.IdentifierName(_proxyToken);
        var param = SyntaxFactory.Parameter(paramName.Identifier).WithType(_proxyType);
        var ctor = SyntaxFactory.ConstructorDeclaration(node.Identifier)
            .WithModifiers(SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword)))
            .WithParameterList(SyntaxFactory.ParameterList().AddParameters(param))
            .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(
                SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    proxyName, paramName
                )
            ))
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        node = node.AddMembers(ctor);
        return node;
    }

    private MethodDeclarationSyntax VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        var obj = _proxyToken.Value;
        var method = node.Identifier.ToFullString();
        var args = SyntaxReceiverHelper.Convert(node.ParameterList);
        node = node.WithExpressionBody(SyntaxFactory.ArrowExpressionClause(
            SyntaxFactory.ParseExpression($"{obj}.{method}{args};\r")
        ));
        return node;
    }

    private PropertyDeclarationSyntax VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        node = node.WithAccessorList(SyntaxFactory.AccessorList(
            SyntaxFactory.List<AccessorDeclarationSyntax>()
                .Add(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration,
                    SyntaxFactory.Block(SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName(_proxyToken)))))
                .Add(SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration,
                    SyntaxFactory.Block(SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        SyntaxFactory.IdentifierName(_proxyToken),
                        SyntaxFactory.IdentifierName("value"))))))
            ));
        return node;
    }
}
