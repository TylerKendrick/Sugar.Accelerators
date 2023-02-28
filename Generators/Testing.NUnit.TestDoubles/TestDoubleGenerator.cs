using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MyNamespace
{
    [Generator]
    public class TestDoubleGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
                return;

            var compilation = context.Compilation;
            var attributeSymbol = compilation.GetTypeByMetadataName(typeof(GenerateTestDoubleAttribute).FullName);

            if (attributeSymbol is null)
                return;

            foreach (var classDeclaration in receiver.CandidateClasses)
            {
                var classSymbol = compilation.GetSemanticModel(classDeclaration.SyntaxTree).GetDeclaredSymbol(classDeclaration);

                if (classSymbol is null || !classSymbol.GetAttributes().Any(x => x.AttributeClass.Name == attributeSymbol.Name))
                    continue;

                var generatedCode = GenerateTestDoubleCode(classSymbol);

                context.AddSource($"{classSymbol.Name}TestDouble.cs", SourceText.From(generatedCode, System.Text.Encoding.UTF8));
            }
        }

        private static string GenerateTestDoubleCode(INamedTypeSymbol classSymbol)
        {
            var usings = new List<string>
            {
                "System",
                "System.Linq",
                "System.Collections.Generic",
                $"using {classSymbol.ContainingNamespace};",
            };

            var className = $"{classSymbol.Name}TestDouble";

            var properties = classSymbol.GetMembers().OfType<IPropertySymbol>()
                .Select(p => $"public {p.Type} {p.Name} {{ get; set; }}");

            var methods = classSymbol.GetMembers().OfType<IMethodSymbol>()
                .Where(m => m.MethodKind == MethodKind.Ordinary && !m.IsStatic)
                .Select(m => GenerateMethodSignature(m));

            var code = $@"
                {string.Join(Environment.NewLine, usings)}
                namespace {classSymbol.ContainingNamespace}.TestDoubles
                {{
                    public class {className} : {classSymbol}
                    {{
                        {string.Join(Environment.NewLine, properties)}
                        {string.Join(Environment.NewLine, methods)}
                    }}
                }}";

            return code;
        }

        private static string GenerateMethodSignature(IMethodSymbol methodSymbol)
        {
            var returnType = methodSymbol.ReturnType;

            var parameters = string.Join(", ", methodSymbol.Parameters.Select(p => $"{p.Type} {p.Name}"));

            return $"public {returnType} {methodSymbol.Name}({parameters}) {{}}";
        }

        private class SyntaxReceiver : ISyntaxReceiver
        {
            public List<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax &&
                    classDeclarationSyntax.AttributeLists.Count > 0)
                {
                    CandidateClasses.Add(classDeclarationSyntax);
                }
            }
        }
    }
}