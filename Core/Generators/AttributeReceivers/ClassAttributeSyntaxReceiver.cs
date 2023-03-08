/// <summary>
/// Syntax receiver that filters class declaration syntax nodes for a specific attribute type.
/// </summary>
/// <typeparam name="TAttribute">The type of attribute to filter for.</typeparam>
public class ClassAttributeSyntaxReceiver<TAttribute>
    : FilteredAttributeSyntaxReceiver<TAttribute, ClassDeclarationSyntax>
    where TAttribute : Attribute
{
}
