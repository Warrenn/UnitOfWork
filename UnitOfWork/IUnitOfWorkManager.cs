namespace UnitOfWork
{
    public interface IUnitOfWorkManager
    {
        IUnitOfWork Create();
        object CurrentContext { get; }
    }
}
