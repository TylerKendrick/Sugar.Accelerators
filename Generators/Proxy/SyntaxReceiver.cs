namespace Sugar.Accelerators.Generators;

public partial class ProxyGenerator
{
    // Syntax receiver to find interface declarations with GenerateProxy attribute
    private class SyntaxReceiver : ISyntaxReceiver
    {
        private List<InterfaceDeclarationSyntax> _declarations = new();
        public IEnumerable<InterfaceDeclarationSyntax> Declarations => _declarations;

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
            _declarations.Add(interfaceDecl);
       }
    }
}