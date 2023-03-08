public class InterfaceAttributeSyntaxReceiver<TAttribute> : FilteredAttributeSyntaxReceiver<TAttribute, InterfaceDeclarationSyntax>
    where TAttribute : Attribute
{
}