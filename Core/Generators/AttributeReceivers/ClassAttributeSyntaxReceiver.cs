public class ClassAttributeSyntaxReceiver<TAttribute> : FilteredAttributeSyntaxReceiver<TAttribute, ClassDeclarationSyntax>
    where TAttribute : Attribute
{
}
