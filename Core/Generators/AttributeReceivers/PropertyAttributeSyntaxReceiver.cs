public class PropertyAttributeSyntaxReceiver<TAttribute> : FilteredAttributeSyntaxReceiver<TAttribute, PropertyDeclarationSyntax>
    where TAttribute : Attribute
{
}