using UnityEngine;
using System.Collections;
using UniRx;

namespace ProcedualLevels.Views
{
    public abstract class ReactiveState<TContext>
    {
        protected TContext Context { get; private set; }
        protected CompositeDisposable Disposable { get; private set; }

        public ReactiveState(TContext context)
        {
            Context = context;
            Disposable = new CompositeDisposable();
        }

        public void Dispose()
        {
            OnDispose();
            Disposable.Dispose();
        }

        public virtual void OnDispose()
        {
        }
    }
}