/// <summary>
/// Syntax receiver that filters field declaration syntax nodes for a specific attribute type.
/// </summary>
/// <typeparam name="TAttribute">The type of attribute to filter for.</typeparam>
public class FieldAttributeSyntaxReceiver<TAttribute>
    : FilteredAttributeSyntaxReceiver<TAttribute, FieldDeclarationSyntax>
    where TAttribute : Attribute
{
}