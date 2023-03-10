namespace Sugar.Accelerators.Generators;

public class DefaultImplementationSyntaxVisitor : CSharpSyntaxRewriter
{
    private readonly SemanticModel _model;
    private readonly TypeSyntax _interfaceType;

    public DefaultImplementationSyntaxVisitor(SemanticModel model, TypeSyntax interfaceType)
    {
        _model = model;
        _interfaceType = interfaceType;
    }

    public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        node = node.AddBaseListTypes(SyntaxFactory.SimpleBaseType(_interfaceType));
        var typeInfo = _model.GetTypeInfo(_interfaceType);
        var members = typeInfo.Type!.GetMembers();
        foreach(var member in members)
        {
            switch(member)
            {
                case IMethodSymbol method:
                    var modifiers = new []
                    {
                        method.IsAbstract ? SyntaxKind.AbstractKeyword : SyntaxKind.None,
                        method.IsAsync ? SyntaxKind.AsyncKeyword : SyntaxKind.None,
                        SyntaxKind.PublicKeyword,
                        SyntaxKind.VirtualKeyword
                    };
                    // var location = node.SyntaxTree.GetLocation(node);
                    // var returnType = method.ReturnType.ToMinimalDisplayString(_model, location);
                    // SyntaxFactory.MethodDeclaration(method.ReturnType, method.Name)
                    break;
                case IPropertySymbol property:
                    break;
                case IEventSymbol @event:
                    break;
                default: break;
            }
        }
        return base.VisitClassDeclaration(node);
    }
}
