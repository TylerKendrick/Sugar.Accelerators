using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Reflection;
using System.Linq;
using NUnit.Framework;
using Sugar.Accelerators.Generators;

namespace GeneratorTests.Tests
{
    [TestFixture]
    public class GeneratorTests
    {
        [Test]
        public void SimpleGeneratorTest()
        {
            // Create the 'input' compilation that the generator will act on
            var inputCompilation = CreateCompilation(@"
namespace MyCode
{
    [Sugar.Accelerators.Generators.GenerateProxyAttribute]
    public partial interface MyInterface
    {
        void Method();
        int MethodWithReturnAndArgs(int arg);
    }
}
");

            // directly create an instance of the generator
            // (Note: in the compiler this is loaded from an assembly, and created via reflection at runtime)
            var generator = new ProxyGenerator();

            // Create the driver that will control the generation, passing in our generator
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            // Run the generation pass
            // (Note: the generator driver itself is immutable, and all calls return an updated version of the driver that you should use for subsequent calls)
            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

            foreach(var diagnostic in diagnostics)
            {
                Console.WriteLine(diagnostic);
            }
            var outputDiagnostics = outputCompilation.GetDiagnostics();
            foreach(var diagnostic in outputDiagnostics)
                Console.WriteLine(diagnostic);
            // We can now assert things about the resulting compilation:
            Assert.That(diagnostics.IsEmpty, Is.True); // there were no diagnostics created by the generators
            Assert.That(outputCompilation.SyntaxTrees.Count(), Is.Not.EqualTo(0)); // we have two syntax trees, the original 'user' provided one, and the one added by the generator
            Assert.That(outputDiagnostics.IsEmpty, Is.True); // verify the compilation with the added source has no diagnostics

            // Or we can look at the results directly:
            var runResult = driver.GetRunResult();

            // The runResult contains the combined results of all generators passed to the driver
            foreach(var diagnostic in runResult.Diagnostics)
                Console.WriteLine(diagnostic);
            Assert.That(runResult.GeneratedTrees.Length, Is.EqualTo(1));
            Assert.That(runResult.Diagnostics.IsEmpty, Is.True);

            // Or you can access the individual results on a by-generator basis
            var generatorResult = runResult.Results[0];
            Assert.That(generatorResult.Generator, Is.EqualTo(generator));
            Assert.That(generatorResult.Diagnostics.IsEmpty, Is.True);
            Assert.That(generatorResult.GeneratedSources.Length, Is.EqualTo(1));
            Assert.That(generatorResult.Exception, Is.Null);
        }

        private static Compilation CreateCompilation(string source)
        {
            var proxyType = typeof(GenerateProxyAttribute);
            MetadataReference Get(string location) => MetadataReference.CreateFromFile(location);
            MetadataReference GetType(Type t) => Get(t.Assembly.Location);
            MetadataReference GetCore(string name)
            {
                var assemblyLocation = typeof(object).Assembly.Location;
                var runtimeDirectory = Path.GetDirectoryName(assemblyLocation);
                var libraryPath = Path.Join(runtimeDirectory, name);
                return Get(libraryPath);
            }
            MetadataReference[] references = new[] {
                GetType(proxyType),
                GetCore("netstandard.dll"),
                GetCore("System.Runtime.dll"),
                GetCore("System.Private.CoreLib.dll"),
            };
            return CSharpCompilation.Create("compilation",
                new[] { CSharpSyntaxTree.ParseText(source) },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }
    }
}
