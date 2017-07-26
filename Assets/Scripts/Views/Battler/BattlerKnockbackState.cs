using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace ProcedualLevels.Views
{
    public abstract class BattlerKnockbackState : ReactiveState<BattlerController>
	{

        public BattlerKnockbackState(BattlerController context) : base(context)
        {
        }

        public override void Subscribe()
        {
            Context.UpdateAsObservable()
                   .Subscribe(x => Control())
                   .AddTo(Disposable);
        }

        public void ChangeState(BattlerKnockbackState state)
        {
            if (state.GetType() == GetType())
            {
                Debug.LogWarning("Transit to same state!");
            }
            Context.KnockbackState = state;
            state.Subscribe();
            Dispose();
        }

        protected abstract void Control();
        public abstract void Knockback(BattlerController against, int power);
    }
}