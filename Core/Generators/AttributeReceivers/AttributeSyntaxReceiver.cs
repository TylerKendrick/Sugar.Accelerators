/// <summary>
/// Receives syntax nodes and filters out the ones that have a specific attribute.
/// </summary>
/// <typeparam name="TAttribute">The type of the attribute to filter for.</typeparam>
public class AttributeSyntaxReceiver<TAttribute> : ISyntaxReceiver where TAttribute : Attribute
{
    private Type _attributeType;
    private List<MemberDeclarationSyntax> _types = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeSyntaxReceiver{TAttribute}"/> class.
    /// </summary>
    public AttributeSyntaxReceiver()
    {
        _attributeType = typeof(TAttribute);
    }

    /// <summary>
    /// Gets the filtered member declaration syntaxes.
    /// </summary>
    public IEnumerable<MemberDeclarationSyntax> Declarations => _types;

    /// <summary>
    /// Visits a syntax node and adds it to the list of filtered syntaxes if it has the target attribute.
    /// </summary>
    /// <param name="syntaxNode">The syntax node to visit.</param>
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        var attrName = _attributeType.FullName;
        if (syntaxNode is MemberDeclarationSyntax memberSyntax)
        {
            if (!Filter(memberSyntax)) return;
            var hasTargetAttribute = memberSyntax
                .AttributeLists
                .SelectMany(x => x.Attributes)
                .Select(x => x.Name)
                .Select(x => x.ToFullString())
                .Contains(attrName);
            if (hasTargetAttribute)
            {
                _types.Add(memberSyntax);
            }
        }
    }

    /// <summary>
    /// Filters a member declaration syntax. By default, it returns false.
    /// </summary>
    /// <param name="memberDeclaration">The member declaration syntax to filter.</param>
    /// <returns>False by default.</returns>
    protected virtual bool Filter(CSharpSyntaxNode memberDeclaration) { return false; }
}
