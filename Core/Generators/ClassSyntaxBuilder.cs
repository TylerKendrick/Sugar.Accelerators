/// <summary>
/// Provides a builder for generating <see cref="ClassDeclarationSyntax"/> objects.
/// </summary>
public class ClassSyntaxBuilder
{
    private readonly string className;
    private readonly SyntaxKind[] modifiers;
    private readonly List<PropertyDeclarationSyntax> properties = new();
    private readonly List<MethodDeclarationSyntax> methods = new();
    private ConstructorDeclarationSyntax constructor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassSyntaxBuilder"/> class with the specified class name and modifiers.
    /// </summary>
    /// <param name="name">The name of the class to build.</param>
    /// <param name="modifiers">The modifiers to apply to the class.</param>
    public ClassSyntaxBuilder(string name, params SyntaxKind[] modifiers)
    {
        className = name;
        this.modifiers = modifiers;
        constructor = CreateConstructor();
    }

    /// <summary>
    /// Adds a property to the class being built.
    /// </summary>
    /// <param name="type">The type of the property.</param>
    /// <param name="name">The name of the property.</param>
    /// <param name="modifiers">The modifiers to apply to the property.</param>
    /// <returns>The current <see cref="ClassSyntaxBuilder"/> instance.</returns>
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

    /// <summary>
    /// Adds a method to the class being built.
    /// </summary>
    /// <param name="name">The name of the method.</param>
    /// <param name="parameters">The method parameters.</param>
    /// <param name="body">The method body.</param>
    /// <param name="returnType">The method return type.</param>
    /// <param name="modifiers">The modifiers to apply to the method.</param>
    /// <returns>The current <see cref="ClassSyntaxBuilder"/> instance.</returns>
    public ClassSyntaxBuilder BuildMethod(string name,
        string[]? parameters = null,
        string body = "{}",
        string returnType = "void",
        params SyntaxKind[] modifiers)
        => BuildMethod(name, parameters, body, returnType, modifiers.Select(SyntaxFactory.Token).ToArray());

    /// <summary>
    /// Adds a method to the class being built.
    /// </summary>
    /// <param name="name">The name of the method.</param>
    /// <param name="parameters">The method parameters.</param>
    /// <param name="body">The method body.</param>
    /// <param name="returnType">The method return type.</param>
    /// <param name="modifiers">The modifiers to apply to the method.</param>
    /// <returns>The current <see cref="ClassSyntaxBuilder"/> instance.</returns>
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

    /// <summary>
    /// Replaces the default constructor with a custom one.
    /// </summary>
    /// <param name="parameters">The constructor parameters.</param>
    /// <param name="body">The constructor body.</param>
    /// <param name="modifiers">The modifiers to apply to the constructor.</param>
    /// <returns>The current <see cref="ClassSyntaxBuilder"/> instance.</returns>
    public ClassSyntaxBuilder BuildConstructor(
        string[] parameters,
        string body,
        params SyntaxKind[] modifiers)
    {
        constructor = CreateConstructor(parameters, body, modifiers);
        return this;
    }

    /// <summary>
    /// Builds the final <see cref="ClassDeclarationSyntax"/> object.
    /// </summary>
    /// <returns>The <see cref="ClassDeclarationSyntax"/> object that was built.</returns>
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
