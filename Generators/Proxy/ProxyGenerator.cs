using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Sugar.Accelerators.Generators.Proxy;
using System;

[AttributeUsage(AttributeTargets.Interface)]
public class GenerateProxyAttribute : Attribute
{
}

[Generator]
public class ProxyGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // Register syntax receiver to find interface declarations with GenerateProxy attribute
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not SyntaxReceiver syntaxReceiver) return;
        var interfaceDeclaration = syntaxReceiver.InterfaceDeclaration;
        var name = interfaceDeclaration!.Identifier.Text;
        // Get the compilation and semantic models for the generated code
        var compilation = context.Compilation;
        var semanticModel = compilation.GetSemanticModel(interfaceDeclaration.SyntaxTree);

        Console.WriteLine("starting generation");
        var body = GenerateProxyClass(interfaceDeclaration, semanticModel);
        Console.WriteLine("wrapping up generation");
        var bodyString = body.NormalizeWhitespace().ToFullString();
        Console.WriteLine(bodyString);
        context.AddSource($"{name}.proxy.cs", SourceText.From(bodyString, System.Text.Encoding.UTF8));
    }

    private ClassDeclarationSyntax GenerateProxyClass(InterfaceDeclarationSyntax interfaceDecl, SemanticModel semanticModel)
    {
        // Create the class declaration for the proxy
        
        var proxyHandle = SyntaxFactory.Identifier("_proxyHandle");
        Console.WriteLine("Creating class def");
        var proxyClass = SyntaxFactory.ClassDeclaration($"{interfaceDecl.Identifier}Proxy")
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(interfaceDecl.Identifier.Text)))
            .AddMembers(SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.ParseTypeName(interfaceDecl.Identifier.NormalizeWhitespace().ToFullString()),
                    SyntaxFactory.SeparatedList(new []{ SyntaxFactory.VariableDeclarator(proxyHandle) })
                )));
        Console.WriteLine("Created class def");

        // Generate the methods for the proxy
        foreach (var methodDecl in interfaceDecl.Members.OfType<MethodDeclarationSyntax>())
        {
            if (methodDecl.Modifiers.Any(SyntaxKind.DefaultKeyword))
            {
                // Exclude methods with default implementations
                continue;
            }

            var methodSymbol = semanticModel.GetDeclaredSymbol(methodDecl);

            // Create the method signature for the proxy
            var methodSignature = SyntaxFactory.MethodDeclaration(
                    methodDecl.ReturnType,
                    methodDecl.Identifier
                )
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.VirtualKeyword)))
                .WithParameterList(methodDecl.ParameterList);

            // Create the method body for the proxy
            var methodBody = GenerateMethodBody(proxyHandle, methodSymbol);

            // Add the method to the proxy class
            var method = methodSignature.WithBody(methodBody);
            Console.WriteLine("Adding method");
            proxyClass = proxyClass.AddMembers(method);
        }

        return proxyClass;
    }

    private BlockSyntax GenerateMethodBody(SyntaxToken fieldSymbol, IMethodSymbol methodSymbol)
    {
        // Create the statements for the method body
        var statements = new List<StatementSyntax>();
        var memberAccess = SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            SyntaxFactory.IdentifierName(fieldSymbol),
            SyntaxFactory.IdentifierName(methodSymbol.Name)
        );
        Console.WriteLine("made a member access expression.");
        // Call the base method on the interface
        var argNodes = methodSymbol.Parameters
            .Select(x => x.Name)
            .Select(SyntaxFactory.IdentifierName)
            .Select(SyntaxFactory.Argument);
        var invocation = SyntaxFactory
            .InvocationExpression(memberAccess);
        Console.WriteLine("Made it past invocation");
        if(argNodes.Any())
        {
            Console.WriteLine("Found arg nodes");
            var separatedList = SyntaxFactory.SeparatedList<ArgumentSyntax>(argNodes);
            var arguments = SyntaxFactory.ArgumentList(separatedList);
            invocation = invocation.WithArgumentList(arguments);
            
            Console.WriteLine("created arg nodes");
        }

        // Return a default value for the method return type
        if (!methodSymbol.ReturnsVoid)
        {
            Console.WriteLine("Writing non-void method");
            var returnStatement = SyntaxFactory.ReturnStatement(invocation);
            statements.Add(returnStatement);
            Console.WriteLine("Wrote non-void method");
        }
        else
        {
            Console.WriteLine("Writing void method");
            var baseMethodCall = SyntaxFactory.ExpressionStatement(invocation);
            statements.Add(baseMethodCall);
            Console.WriteLine("Wrote void method");
        }

        // Create the method body block
        var methodBody = SyntaxFactory.Block(statements);
        return methodBody;
    }

    private class AttributeSyntaxReceiver : ISyntaxReceiver
    {
        private Type _attributeType;
        private List<TypeDeclarationSyntax> _types = new();

        public AttributeSyntaxReceiver(Attribute attribute)
        {
            _attributeType = attribute.GetType();
        }

        public IEnumerable<TypeDeclarationSyntax> TypeDeclarations => _types;

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // Find interface declarations
            if (syntaxNode is not TypeDeclarationSyntax typeDecl) return;
            var attrs = typeDecl
                .AttributeLists
                .SelectMany(x => x.Attributes)
                .Select(x => x.Name)
                .Select(x => x.NormalizeWhitespace())
                .Select(x => x.ToFullString());
            var attrName = _attributeType.FullName;
            if(!attrs.Contains(attrName)) return;

            _types.Add(typeDecl);
       }
    }
    // Syntax receiver to find interface declarations with GenerateProxy attribute
    private class SyntaxReceiver : ISyntaxReceiver
    {
        public InterfaceDeclarationSyntax? InterfaceDeclaration { get; private set; }

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // Find interface declarations
            if (syntaxNode is not InterfaceDeclarationSyntax interfaceDecl) return;
            var attrs = interfaceDecl
                .AttributeLists
                .SelectMany(x => x.Attributes)
                .Select(x => x.Name)
                .Select(x => x.NormalizeWhitespace())
                .Select(x => x.ToFullString());
            foreach(var attr in attrs)
                Console.WriteLine(attr);
            var attrName = typeof(GenerateProxyAttribute).FullName;
            if(!attrs.Contains(attrName)) return;
            // TODO: make into a list.
            InterfaceDeclaration = interfaceDecl;
       }
    }
}