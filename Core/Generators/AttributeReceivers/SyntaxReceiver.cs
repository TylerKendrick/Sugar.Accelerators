/// <summary>
/// Receives syntax nodes and filters out the ones that have a specific attribute.
/// </summary>
/// <typeparam name="TSyntaxNode">The type of the node to filter for.</typeparam>
public class SyntaxReceiver<TSyntaxNode> : ISyntaxReceiver
    where TSyntaxNode : SyntaxNode
{
    private List<TSyntaxNode> _nodes = new();

    /// <summary>
    /// Gets the filtered member declaration syntaxes.
    /// </summary>
    public IEnumerable<TSyntaxNode> Nodes => _nodes;

    /// <summary>
    /// Visits a syntax node and adds it to the list of filtered syntaxes if it has the target attribute.
    /// </summary>
    /// <param name="syntaxNode">The syntax node to visit.</param>
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not TSyntaxNode targetSyntax) return;
        if (Filter(targetSyntax)) return;
        _nodes.Add(targetSyntax);
    }

    /// <summary>
    /// Filters a member declaration syntax. By default, it returns false.
    /// </summary>
    /// <param name="memberDeclaration">The member declaration syntax to filter.</param>
    /// <returns>False by default.</returns>
    protected virtual bool Filter(TSyntaxNode memberDeclaration) { return false; }
}
