using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UniRx.Triggers;

namespace ProcedualLevels.Views
{
    public class BattlerKnockbackStateKnockback : BattlerKnockbackState
    {
        private int Power { get; set; }

        public BattlerKnockbackStateKnockback(BattlerController context, int power) : base(context)
        {
            Power = power;
        }

        public override void Subscribe()
        {
            base.Subscribe();
            Observable.Timer(TimeSpan.FromMilliseconds(500))
                      .Subscribe(x =>
            {
                ChangeState(new BattlerKnockbackStateNeutral(Context));
            });

            Context.OnCollisionStay2DAsObservable()
                   .Select(x => x.gameObject.GetComponent<BattlerController>())
                   .Where(x => x != null)
                   .Subscribe(x =>
            {
                x.Knockback(Context, Power);
            })
                   .AddTo(Disposable);
        }

        public override void Knockback(BattlerController against, int power)
        {
        }

        protected override void Control()
        {
        }
    }
}