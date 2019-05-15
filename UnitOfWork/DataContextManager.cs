using System;
using System.Collections.Generic;
using System.Threading;

namespace UnitOfWork
{
    public class DataContextManager<TContext> : IDataContextManager<TContext> where TContext : class, IDisposable
    {
        protected readonly Func<TContext> contextFactory;
        protected readonly Func<Action, Func<TContext>, IUnitOfWork> unitOfWorkFactory;

        [ThreadStatic]
        private static AsyncLocal<Stack<TContext>> _localStack;

        protected Stack<TContext> ContextStack
        {
            get
            {
                if (_localStack?.Value == null) _localStack = new AsyncLocal<Stack<TContext>> { Value = new Stack<TContext>() };
                return _localStack.Value;
            }
        }

        public TContext CurrentContext { get => (ContextStack.Count > 0) ? ContextStack.Peek() : CreateNewDataContext(); }

        public DataContextManager(Func<TContext> contextFactory, Func<Action, Func<TContext>, IUnitOfWork> unitOfWorkFactory = null)
        {
            if (unitOfWorkFactory == null && !typeof(ISaveChanges).IsAssignableFrom(typeof(TContext)))
            {
                throw new Exception("Custom Data Context must Implement ISaveChanges interface");
            }

            this.contextFactory = contextFactory;
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        protected virtual TContext ContextFactory() => contextFactory();

        protected TContext CreateNewDataContext()
        {
            var repository = ContextFactory();
            ContextStack.Push(repository);
            return repository;
        }

        public virtual IUnitOfWork Create()
        {
            CreateNewDataContext();
            var unitOfWork = (unitOfWorkFactory == null)
                ? new UnitOfWork<TContext>(CloseDataContext, () => CurrentContext)
                : unitOfWorkFactory(CloseDataContext, () => CurrentContext);
            return unitOfWork;
        }

        protected virtual void CloseDataContext()
        {
            CurrentContext?.Dispose();
            ContextStack.Pop();
        }
    }
}
