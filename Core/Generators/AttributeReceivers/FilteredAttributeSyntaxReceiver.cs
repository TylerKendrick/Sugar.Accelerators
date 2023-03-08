public class FilteredAttributeSyntaxReceiver<TAttribute, TTypeFilter> : AttributeSyntaxReceiver<TAttribute>
    where TAttribute : Attribute
    where TTypeFilter : CSharpSyntaxNode
{
    protected sealed override bool Filter(CSharpSyntaxNode memberDeclaration)
    {
        if(memberDeclaration is TTypeFilter typeDeclaration)
        {
            OnFilter(typeDeclaration);
            return true;
        }
        return false;
    }
    protected virtual void OnFilter(TTypeFilter memberDeclaration) { }
}