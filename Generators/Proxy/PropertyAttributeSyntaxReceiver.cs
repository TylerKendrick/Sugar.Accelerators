using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sugar.Accelerators.Generators.Proxy;
using System;
using Microsoft.CodeAnalysis.CSharp;

public partial class ProxyGenerator
{
    private class PropertyAttributeSyntaxReceiver<TAttribute> : AttributeSyntaxReceiver<TAttribute>
        where TAttribute : Attribute
    {
        protected override bool Filter(CSharpSyntaxNode memberDeclaration) => memberDeclaration is PropertyDeclarationSyntax;
    }
}