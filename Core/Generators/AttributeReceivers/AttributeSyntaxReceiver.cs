/// <summary>
/// Receives syntax nodes and filters out the ones that have a specific attribute.
/// </summary>
/// <typeparam name="TAttribute">The type of the attribute to filter for.</typeparam>
public class AttributeSyntaxReceiver<TAttribute> : SyntaxReceiver<MemberDeclarationSyntax>
    where TAttribute : Attribute
{
    private Type _attributeType;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeSyntaxReceiver{TAttribute}"/> class.
    /// </summary>
    public AttributeSyntaxReceiver()
    {
        _attributeType = typeof(TAttribute);
    }

    /// <summary>
    /// Filters a member declaration syntax. By default, it returns false.
    /// </summary>
    /// <param name="memberSyntax">The member declaration syntax to filter.</param>
    /// <returns>False by default.</returns>
    protected override bool Filter(MemberDeclarationSyntax memberSyntax)
        => base.Filter(memberSyntax) && !memberSyntax
            .AttributeLists
            .SelectMany(x => x.Attributes)
            .Select(x => x.Name)
            .Select(x => x.ToFullString())
            .Contains(_attributeType.FullName);
}
