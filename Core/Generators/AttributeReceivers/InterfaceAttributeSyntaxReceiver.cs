/// <summary>
/// Syntax receiver that filters interface declaration syntax nodes for a specific attribute type.
/// </summary>
/// <typeparam name="TAttribute">The type of attribute to filter for.</typeparam>
public class InterfaceAttributeSyntaxReceiver<TAttribute>
: FilteredAttributeSyntaxReceiver<TAttribute, InterfaceDeclarationSyntax>
    where TAttribute : Attribute
{
}