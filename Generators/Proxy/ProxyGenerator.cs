using Microsoft.CodeAnalysis.Text;

namespace Sugar.Accelerators.Generators.Proxy;
using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

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
        var constructor = GenerateConstructor(typeDecl, proxyClassIdentifier, out var constructorFields);
        var methods = GenerateMethods(typeDecl, proxyHandle, proxyClass);
        return proxyClass
            .AddMembers(constructor)
            .AddMembers(constructorFields)
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
        out FieldDeclarationSyntax[] fields)
    {
        var @params = SyntaxFactory.ParseParameterList(
                $"({typeDecl.NormalizeWhitespace().ToFullString()} proxyHandle)");
        var proxyHandleField = SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(
            SyntaxFactory.ParseTypeName(typeDecl.Text),
            SyntaxFactory.SeparatedList<VariableDeclaratorSyntax>()
                .Add(SyntaxFactory.VariableDeclarator(typeDecl))
            ));
        fields = new[] { proxyHandleField };
        return SyntaxFactory.ConstructorDeclaration(proxyClassIdentifier)
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword)))
            .WithBody(SyntaxFactory.Block(SyntaxFactory.ParseStatement("_proxyHandle = proxyHandle;")))
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