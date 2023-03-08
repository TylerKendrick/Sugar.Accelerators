/// <summary>
/// Attribute syntax receiver that filters out attribute declarations that are not applied to a specific type filter.
/// </summary>
/// <typeparam name="TAttribute">The attribute type to filter for.</typeparam>
/// <typeparam name="TTypeFilter">The type filter to apply.</typeparam>
public class FilteredAttributeSyntaxReceiver<TAttribute, TTypeFilter> : AttributeSyntaxReceiver<TAttribute>
    where TAttribute : Attribute
    where TTypeFilter : CSharpSyntaxNode
{
    /// <summary>
    /// Overrides the Filter method to check if the member declaration is of the specified type filter.
    /// </summary>
    /// <param name="memberDeclaration">The member declaration syntax node.</param>
    /// <returns>A boolean value indicating whether the member declaration is of the specified type filter.</returns>
    protected sealed override bool Filter(CSharpSyntaxNode memberDeclaration)
    {
        if(memberDeclaration is TTypeFilter typeDeclaration)
        {
            OnFilter(typeDeclaration);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Called when the member declaration is of the specified type filter.
    /// </summary>
    /// <param name="memberDeclaration">The member declaration syntax node.</param>
    protected virtual void OnFilter(TTypeFilter memberDeclaration) { }
}
