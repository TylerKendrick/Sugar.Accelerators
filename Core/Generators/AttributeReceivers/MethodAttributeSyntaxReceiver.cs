/// <summary>
/// Syntax receiver that filters method declaration syntax nodes for a specific attribute type.
/// </summary>
/// <typeparam name="TAttribute">The type of attribute to filter for.</typeparam>
public class MethodAttributeSyntaxReceiver<TAttribute>
    : FilteredAttributeSyntaxReceiver<TAttribute, MethodDeclarationSyntax>
    where TAttribute : Attribute
{
}