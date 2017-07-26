using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UniRx.Triggers;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// キャラクターがノックバックをしている状態の機能を提供するクラス。
    /// </summary>
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

            // 一定時間で通常の状態に戻る
            Observable.Timer(TimeSpan.FromMilliseconds(500))
                      .Subscribe(x =>
            {
                ChangeState(new BattlerKnockbackStateNeutral(Context));
            })
                      .AddTo(Disposable);

            // 他のキャラクターにぶつかったらそれも押し飛ばす
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