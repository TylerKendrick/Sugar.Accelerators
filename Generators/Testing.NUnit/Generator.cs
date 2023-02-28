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
        public TargetTestsForAssemblyAttribute()
        {
            Assembly =  Assembly.GetCallingAssembly();
        }
        public Assembly Assembly
        {
            get; private set;
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
            var types =  context.Compilation.Assembly.GetAttributes()
                .Where(attr => attr.AttributeClass!.Name == nameof(TargetTestsForAssemblyAttribute))
                .Select(x => x.AttributeClass.GetMembers("Assembly"))
                .Cast<Assembly>()
                .SelectMany(x => x.GetTypes());

            foreach (var type in types)
            {                
                var properties = type.GetProperties();
                var methods = type.GetMethods();
                var testCode = GenerateTestCode(type);
                
                foreach(var property in properties)
                {
                    var propTestCode = GeneratePropertyTestCode(type, property);
                    context.AddSource($"{type.Name}.{property.Name}.tests.g.cs", propTestCode);
                }
                context.AddSource($"{type.Name}.tests.g.cs", testCode);
            }
        }

        private string GeneratePropertyTestCode(Type type, PropertyInfo property)
        {
            var testName = type.Name;
            Console.WriteLine($"Generated: {type.Namespace}.Tests.{testName}Tests");
            return $@"
                using NUnit.Framework;
                namespace {type.Namespace}.Tests
                {{
                    public partial class {testName}Tests
                    {{
                        [Test]
                        public void {property.Name}GetMethodTests()
                        {{
                            var obj = new {type.FullName}();
                            // TODO: Add test code here
                            Assert.Pass();
                        }}

                        [Test]
                        public void {property.Name}SetMethodTests({property.PropertyType.FullName} @value)
                        {{
                            var obj = new {type.FullName}();
                            // TODO: Add test code here
                            Assert.Pass();
                        }}

                        private partial void GetMethodInternals({type.FullName} concern);
                    }}
                }}";
        }

        private string GenerateTestCode(Type type)
        {
            var properties = type.GetProperties();
            var methods = type.GetMethods();

            var testName = type.Name;
            return $@"
                using NUnit.Framework;
                namespace {type.Namespace}.Tests
                {{
                    public partial class {testName}Tests
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
