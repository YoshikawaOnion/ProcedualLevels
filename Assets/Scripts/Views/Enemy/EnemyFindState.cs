using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public abstract class EnemyFindState : ReactiveState<EnemyController>
    {
        public EnemyFindState(EnemyController context) : base(context)
        {
        }

        public void ChangeState(EnemyFindState state)
        {
            Context.FindState = state;
            state.Subscribe();
            Dispose();
        }

        public abstract void Control();
    }
}