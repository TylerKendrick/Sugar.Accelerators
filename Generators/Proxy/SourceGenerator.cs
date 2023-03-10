namespace Sugar.Accelerators.Generators;

public partial class SourceGenerator<TReceiver> : ISourceGenerator
    where TReceiver : ISyntaxReceiver, new()
{
    private readonly Func<TReceiver> _receiverFactory;
    protected SourceGenerator(Func<TReceiver>? receiverFactory = null)
    {
        _receiverFactory = receiverFactory ?? (() => new());
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        Console.WriteLine("INITIALIZING GENERATOR");
        context.RegisterForSyntaxNotifications(() => _receiverFactory());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        Console.WriteLine("EXECUTING GENERATOR");
        switch (context.SyntaxReceiver)
        {
            case TReceiver receiver:
                Console.WriteLine("DELEGATING EXECUTION");
                OnExecute(context, receiver);
                break;
            default: return;
        }
    }

    protected virtual void OnExecute(GeneratorExecutionContext context, TReceiver receiver)
    {
        Console.WriteLine("BASE GENERATOR EXECUTING");
    }
}
