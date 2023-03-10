namespace Sugar.Accelerators.Generators;

[Generator]
public partial class ProxyGenerator : SourceGenerator<InterfaceAttributeSyntaxReceiver<GenerateProxyAttribute>>
{
    protected override void OnExecute(
        GeneratorExecutionContext context,
        InterfaceAttributeSyntaxReceiver<GenerateProxyAttribute> interfaceSyntax)
    {
        foreach(InterfaceDeclarationSyntax interfaceDeclaration in interfaceSyntax.Nodes)
        {
            string name = interfaceDeclaration!.Identifier.Text;
            var identifier = SyntaxFactory.Identifier($"{name}Proxy");
            var classSyntax = SyntaxFactory.ClassDeclaration(identifier)
                .WithModifiers(SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .Accept(new ProxySyntaxVisitor(SyntaxFactory.ParseTypeName(name)));
            Console.WriteLine(classSyntax?.NormalizeWhitespace().ToFullString() ?? "No class syntax");

            var classBuilder = new ClassSyntaxBuilder($"{name}Proxy");
            classBuilder.BuildInterfaces(name);
            var typeNamespace = SyntaxReceiverHelper.GetNode<NamespaceDeclarationSyntax>(interfaceDeclaration);
            var usingBlock = SyntaxFactory.UsingDirective(
                SyntaxFactory.ParseName(typeNamespace.Name.ToFullString()));
            var body = GenerateProxyClass(classBuilder, interfaceDeclaration);
            var definitionString = string.Join("\n",
                usingBlock.NormalizeWhitespace().ToFullString(),
                body.NormalizeWhitespace().ToFullString());
            Console.WriteLine(definitionString);
            context.AddSource($"{name}.proxy.cs", SourceText.From(definitionString, System.Text.Encoding.UTF8));
        }
    }
    
    private ClassDeclarationSyntax GenerateProxyClass(ClassSyntaxBuilder builder,
        TypeDeclarationSyntax typeDecl)
    {
        GenerateConstructor(builder, typeDecl.Identifier);
        GenerateMethods(builder, typeDecl);
        //TODO: Add Properties.
        return builder.Build();
    }
    private void GenerateMethods(
        ClassSyntaxBuilder builder,
        TypeDeclarationSyntax typeDecl)
        {
            foreach(MethodDeclarationSyntax method in typeDecl.Members)
            {
                var methodName = method.Identifier.ToFullString();
                var returnType = method.ReturnType.ToFullString().ToLowerInvariant().Trim();
                var returnStatement = returnType == "void" ? "" : "return ";
                var args = SyntaxReceiverHelper.Convert(method.ParameterList).ToFullString();
                var modifiers = new[]{SyntaxKind.PublicKeyword, SyntaxKind.VirtualKeyword}
                    .Select(SyntaxFactory.Token);
                modifiers = modifiers.Concat(method.Modifiers.AsEnumerable());
                builder.BuildMethod(method.Identifier.ToFullString(),
                    method.ParameterList.Parameters.Select(x => x.ToFullString()).ToArray(),
                    $"{{ {returnStatement}_proxyHandle.{methodName}{args};\n }}", returnType,
                    modifiers.ToArray());
            }
        }

    private static void GenerateConstructor(
        ClassSyntaxBuilder builder,
        SyntaxToken typeDecl)
    {
        var proxyHandleIdentifier = "_proxyHandle";
        builder.BuildField(typeDecl.Text, proxyHandleIdentifier);

        var @params = new [] {
            (type: typeDecl.NormalizeWhitespace().ToFullString(), name: "proxyHandle")
        };
        var body = SyntaxFactory.Block(SyntaxFactory.ParseStatement($"{proxyHandleIdentifier} = proxyHandle;")).ToFullString();
        builder.BuildConstructor(@params, body);
    }
}