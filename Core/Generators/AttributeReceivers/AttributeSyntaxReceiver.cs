public class AttributeSyntaxReceiver<TAttribute> : ISyntaxReceiver
    where TAttribute : Attribute
{
    private Type _attributeType;
    private List<MemberDeclarationSyntax> _types = new();

    public AttributeSyntaxReceiver()
    {
        _attributeType = typeof(TAttribute);
    }

    public IEnumerable<MemberDeclarationSyntax> Declarations => _types;

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        var attrName = _attributeType.FullName;
        if(syntaxNode is MemberDeclarationSyntax memberSyntax)
        {
            if(!Filter(memberSyntax)) return;
            var hasTargetAttribute = memberSyntax
                .AttributeLists
                .SelectMany(x => x.Attributes)
                .Select(x => x.Name)
                .Select(x => x.ToFullString())
                .Contains(attrName);
            if(hasTargetAttribute)
            {
                _types.Add(memberSyntax);
            }
        }
    }

    protected virtual bool Filter(CSharpSyntaxNode memberDeclaration) { return false; }
}