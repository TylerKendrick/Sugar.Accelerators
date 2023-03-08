public class MethodAttributeSyntaxReceiver<TAttribute> : FilteredAttributeSyntaxReceiver<TAttribute, MethodDeclarationSyntax>
    where TAttribute : Attribute
{
}