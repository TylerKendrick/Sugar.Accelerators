/// <summary>
/// Syntax receiver that filters property declaration syntax nodes for a specific attribute type.
/// </summary>
/// <typeparam name="TAttribute">The type of attribute to filter for.</typeparam>
public class PropertyAttributeSyntaxReceiver<TAttribute> 
    : FilteredAttributeSyntaxReceiver<TAttribute, PropertyDeclarationSyntax>
    where TAttribute : Attribute
{
}