using UnityEngine;
using System.Collections;
using UniRx;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// 非同期に処理を行う状態の基底クラス。
    /// </summary>
    public abstract class ReactiveState<TContext>
    {
        protected TContext Context { get; private set; }
        protected CompositeDisposable Disposable { get; private set; }

        public ReactiveState(TContext context)
        {
            Context = context;
            Disposable = new CompositeDisposable();
        }

        /// <summary>
        /// この状態の処理を終了します。
        /// </summary>
        public void Dispose()
        {
            OnDispose();
            Disposable.Dispose();
        }

        /// <summary>
        /// この状態の処理を開始します。
        /// </summary>
        public virtual void Subscribe()
        {
        }

        public virtual void OnDispose()
        {
        }
    }
}