namespace UnitOfWork
{
    public interface IDataContextManager<TContext>
    {
        IUnitOfWork Create();
        TContext CurrentContext { get; }
    }
}
