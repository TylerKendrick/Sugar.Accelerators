public class ClassSyntaxBuilder
{
    private readonly string className;
    private readonly SyntaxKind[] modifiers;
    private readonly List<PropertyDeclarationSyntax> properties = new List<PropertyDeclarationSyntax>();
    private readonly List<MethodDeclarationSyntax> methods = new List<MethodDeclarationSyntax>();
    private ConstructorDeclarationSyntax constructor;

    public ClassSyntaxBuilder(string name, params SyntaxKind[] modifiers)
    {
        className = name;
        this.modifiers = modifiers;
        constructor = CreateConstructor();
    }

    public ClassSyntaxBuilder BuildProperty(string type, string name, params SyntaxKind[] modifiers)
    {
        modifiers = modifiers ?? new[]{SyntaxKind.PublicKeyword};
        var tokens = modifiers.Select(SyntaxFactory.Token).ToArray();
        var property = SyntaxFactory.PropertyDeclaration(
            SyntaxFactory.ParseTypeName(type),
            name
        ).AddModifiers(tokens)
         .AddAccessorListAccessors(
            SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                         .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))
         .AddAccessorListAccessors(
            SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                         .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

        properties.Add(property);
        return this;
    }

    public ClassSyntaxBuilder BuildMethod(string name,
        string[]? parameters = null,
        string body = "{}",
        string returnType = "void",
        params SyntaxKind[] modifiers)
        => BuildMethod(name, parameters, body, returnType, modifiers.Select(SyntaxFactory.Token).ToArray());
    public ClassSyntaxBuilder BuildMethod(string name,
        string[]? parameters = null,
        string body = "{}",
        string returnType = "void",
        params SyntaxToken[] modifiers)
    {
        parameters = parameters ?? new string[0];
        var parameterList = SyntaxFactory.ParameterList(
            SyntaxFactory.SeparatedList(parameters.Select(param => 
                SyntaxFactory.Parameter(
                    default(SyntaxList<AttributeListSyntax>),
                    SyntaxFactory.TokenList(),
                    SyntaxFactory.ParseTypeName(param.Split(' ')[0]),
                    SyntaxFactory.Identifier(param.Split(' ')[1]),
                    null
                )
            ))
        );

        var method = SyntaxFactory.MethodDeclaration(
            SyntaxFactory.ParseTypeName(returnType),
            name
        ).AddModifiers(modifiers)
         .WithParameterList(parameterList)
         .WithBody(SyntaxFactory.ParseStatement(body) as BlockSyntax);

        methods.Add(method);
        return this;
    }

    private ConstructorDeclarationSyntax CreateConstructor(
        string[]? parameters = null,
        string body = "{}",
        params SyntaxKind[] modifiers)
    {
        modifiers = modifiers ?? new[]{SyntaxKind.PrivateKeyword};
        var tokens = modifiers.Select(SyntaxFactory.Token).ToArray();
        parameters = parameters ?? new string[0];
        var parameterList = SyntaxFactory.ParameterList(
            SyntaxFactory.SeparatedList(parameters.Select(param => 
                SyntaxFactory.Parameter(
                    default(SyntaxList<AttributeListSyntax>),
                    SyntaxFactory.TokenList(),
                    SyntaxFactory.ParseTypeName(param.Split(' ')[0]),
                    SyntaxFactory.Identifier(param.Split(' ')[1]),
                    null
                )
            ))
        );

        return SyntaxFactory.ConstructorDeclaration(className)
            .AddModifiers(tokens)
            .WithParameterList(parameterList)
            .WithBody(SyntaxFactory.ParseStatement(body) as BlockSyntax);
    }
    public ClassSyntaxBuilder BuildConstructor(
        string[] parameters,
        string body,
        params SyntaxKind[] modifiers)
    {
        constructor = CreateConstructor(parameters, body, modifiers);
        return this;
    }

    public ClassDeclarationSyntax Build()
        => SyntaxFactory.ClassDeclaration(className)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddMembers(properties.ToArray())
            .AddMembers(constructor)
            .AddMembers(methods.ToArray());

    internal void BuildMethod(MethodDeclarationSyntax method)
        => BuildMethod(method.Identifier.ToFullString(),
            method.ParameterList.Parameters.Select(x => x.ToFullString()).ToArray(),
            method.Body!.ToFullString(),
            method.ReturnType.ToFullString(),
            method.Modifiers.ToArray());
}