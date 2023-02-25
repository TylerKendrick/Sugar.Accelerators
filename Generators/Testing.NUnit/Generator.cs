using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MyNamespace
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class TargetTestsForAssemblyAttribute : Attribute
    {
        public TargetTestsForAssemblyAttribute(string assemblyName)
        {
            AssemblyName = assemblyName;
        }

        public string AssemblyName { get; }
        public Assembly Assembly
        {
            get { return Assembly.Load(AssemblyName); }
        }
    }

    [Generator]
    public class NUnitTestsGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new NUnitTestsSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is NUnitTestsSyntaxReceiver receiver))
                return;

            foreach (var targetAssembly in context.Compilation.Assembly.GetAttributes()
                    .Where(attr => attr.AttributeClass.Name == "TargetTestsForAssemblyAttribute"))
            {
                var assembly = ((string)targetAssembly.ConstructorArguments[0].Value);
                var referencedAssembly = Assembly.Load(assembly);
                var types = referencedAssembly.GetTypes();

                foreach (var type in types)
                {
                    var testName = $"{type.Name}Tests";
                    var testCode = GenerateTestCode(type, testName);
                    context.AddSource($"{type.Name}Tests.generated.cs", testCode);
                }
            }
        }

        private string GenerateTestCode(Type type, string testName)
        {
            return $@"
                using NUnit.Framework;
                namespace {type.Namespace}.Tests
                {{
                    public class {testName}
                    {{
                        [Test]
                        public void Test1()
                        {{
                            var obj = new {type.Name}();
                            // TODO: Add test code here
                            Assert.Pass();
                        }}
                    }}
                }}";
        }
    }

    internal class NUnitTestsSyntaxReceiver : ISyntaxReceiver
    {
        public List<TypeDeclarationSyntax> CandidateTypes { get; } = new List<TypeDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is TypeDeclarationSyntax typeDeclarationSyntax && typeDeclarationSyntax.AttributeLists.Count > 0)
            {
                CandidateTypes.Add(typeDeclarationSyntax);
            }
        }
    }
}
