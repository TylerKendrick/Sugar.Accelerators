using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

/// <summary>
/// Analyzer that checks if a project is referencing another project named "Project.Abstractions".
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ProjectReferenceAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// The diagnostic rule ID for this analyzer.
    /// </summary>
    private const string RuleId = "SUGAR001";

    /// <summary>
    /// The title of the diagnostic produced by this analyzer.
    /// </summary>
    private const string Title = "Project reference analyzer";

    /// <summary>
    /// The message format of the diagnostic produced by this analyzer.
    /// </summary>
    private const string MessageFormat = "Project should reference '%Project%.Abstractions'.";

    /// <summary>
    /// The diagnostic category for this analyzer.
    /// </summary>
    private const string Category = "Usage";

    /// <summary>
    /// The diagnostic rule produced by this analyzer.
    /// </summary>
    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        RuleId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );

    /// <summary>
    /// The list of supported diagnostics for this analyzer.
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);

    /// <summary>
    /// Initializes the analysis context for this analyzer.
    /// </summary>
    /// <param name="context">The analysis context to initialize.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.UsingDirective);
    }

    /// <summary>
    /// Analyzes a syntax node to check for project references.
    /// </summary>
    /// <param name="context">The syntax node analysis context.</param>
    private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
    {
        if (context.Compilation.AssemblyName.EndsWith(".Application") &&
            !context.Compilation.ReferencedAssemblyNames.Any(a => a.Name.EndsWith(".Abstractions")))
        {
            var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
