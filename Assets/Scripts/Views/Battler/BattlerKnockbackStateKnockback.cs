using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public class BattlerKnockbackStateKnockback : BattlerKnockbackState
    {
        public BattlerKnockbackStateKnockback(BattlerController context) : base(context)
        {
        }

        public override void Subscribe()
        {
            base.Subscribe();
            Observable.Timer(TimeSpan.FromMilliseconds(500))
                      .Subscribe(x =>
            {
                ChangeState(new BattlerKnockbackStateNeutral(Context));
            });
        }

        public override void Knockback(BattlerController against, int power)
        {
        }

        protected override void Control()
        {
        }
    }
}