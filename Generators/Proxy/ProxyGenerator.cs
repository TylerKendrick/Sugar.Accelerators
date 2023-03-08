using System.Collections.Generic;
using Humanizer;
namespace Sugar.Accelerators.Generators;

[Generator]
public partial class ProxyGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // Register syntax receiver to find interface declarations with GenerateProxy attribute
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not SyntaxReceiver syntaxReceiver) return;
        foreach(InterfaceDeclarationSyntax interfaceDeclaration in syntaxReceiver.Declarations)
        {
            var name = interfaceDeclaration!.Identifier.Text;
            // Get the compilation and semantic models for the generated code
            var compilation = context.Compilation;
            var semanticModel = compilation.GetSemanticModel(interfaceDeclaration.SyntaxTree);
            
            Console.WriteLine("starting generation");
            var typeNamespace = SyntaxReceiverHelper.GetNode<NamespaceDeclarationSyntax>(interfaceDeclaration);
            var usingBlock = SyntaxFactory.UsingDirective(
                SyntaxFactory.ParseName(typeNamespace.Name.ToFullString()));
            var body = GenerateProxyClass(interfaceDeclaration, semanticModel);
            Console.WriteLine("wrapping up generation");
            var definitionString = string.Join("\n",
                usingBlock.NormalizeWhitespace().ToFullString(),
                body.NormalizeWhitespace().ToFullString());
            Console.WriteLine(definitionString);
            context.AddSource($"{name}.proxy.cs", SourceText.From(definitionString, System.Text.Encoding.UTF8));
        }
    }

    private ClassDeclarationSyntax GenerateProxyClass(TypeDeclarationSyntax typeDecl, SemanticModel semanticModel)
    {
        // Create the class declaration for the proxy
        var proxyClassName = $"{typeDecl.Identifier}Proxy";
        var proxyClassIdentifier = SyntaxFactory.Identifier(proxyClassName);
        var proxyClass = SyntaxFactory.ClassDeclaration(proxyClassIdentifier)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(typeDecl.Identifier.Text)));
        var constructor = GenerateConstructor(typeDecl.Identifier, proxyClassIdentifier, out var proxyHandle);
        var proxyFieldName = proxyHandle.Declaration.Variables
            .Select(x => x.Identifier)
            .First();
        var methods = GenerateMethods(typeDecl, proxyFieldName, proxyClass);
        return proxyClass
            .AddMembers(constructor)
            .AddMembers(proxyHandle)
            .AddMembers(methods);
    }

    private MethodDeclarationSyntax[] GenerateMethods(
        TypeDeclarationSyntax typeDecl,
        SyntaxToken proxyHandle,
        ClassDeclarationSyntax proxyClass)
        => typeDecl.Members
            .OfType<MethodDeclarationSyntax>()
            .Select(method => GenerateMethod(proxyHandle, method))
            .ToArray();

    private static ConstructorDeclarationSyntax GenerateConstructor(
        SyntaxToken typeDecl,
        SyntaxToken proxyClassIdentifier,
        out FieldDeclarationSyntax proxyField)
    {
        var @params = SyntaxFactory.ParseParameterList(
                $"({typeDecl.NormalizeWhitespace().ToFullString()} proxyHandle)");
        var proxyHandleIdentifier = $"_{typeDecl.Text.Camelize()}";
        var proxyHandleField = SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(
            SyntaxFactory.ParseTypeName(typeDecl.Text),
            SyntaxFactory.SeparatedList<VariableDeclaratorSyntax>()
                .Add(SyntaxFactory.VariableDeclarator(proxyHandleIdentifier))
            ));
        proxyField = proxyHandleField;
        return SyntaxFactory.ConstructorDeclaration(proxyClassIdentifier)
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword)))
            .WithBody(SyntaxFactory.Block(SyntaxFactory.ParseStatement($"{proxyHandleIdentifier} = proxyHandle;")))
            .WithParameterList(@params);
    }


    private MethodDeclarationSyntax GenerateMethod(
        SyntaxToken proxyHandle,
        MethodDeclarationSyntax methodDecl)
            => SyntaxReceiverHelper.CreateProxy(
                proxyHandle, methodDecl.AddModifiers(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.VirtualKeyword)));
}