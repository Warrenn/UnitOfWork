using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UnitOfWork
{
    public partial class UnitOfWorkManager : IUnitOfWorkManager
    {
        private static readonly AsyncLocal<Stack<object>> localStack = new AsyncLocal<Stack<object>>();

        static UnitOfWorkManager()
        {
            localStack.Value = new Stack<object>();
        }

        private readonly Func<object> repositoryFactory;
        private readonly Func<object, Task> saveAsync;

        public UnitOfWorkManager(Func<object> repositoryFactory, Func<object, Task> saveAsync)
        {
            this.repositoryFactory = repositoryFactory;
            this.saveAsync = saveAsync;
        }

        public object CurrentContext { get => (localStack.Value.Count > 0) ? localStack.Value.Peek() : CreateNewRepository(); }

        private object CreateNewRepository()
        {
            var repository = repositoryFactory();
            localStack.Value.Push(repository);
            return repository;
        }

        public IUnitOfWork Create()
        {
            CreateNewRepository();
            return new UnitOfWork(() => PopRepositoryOffStack(), () => saveAsync(CurrentContext));
        }

        private void PopRepositoryOffStack()
        {
            localStack.Value.Pop();
        }
    }
}
