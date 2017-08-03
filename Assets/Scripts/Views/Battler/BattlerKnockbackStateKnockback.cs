using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UniRx.Triggers;
using ProcedualLevels.Models;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// キャラクターがノックバックをしている状態の機能を提供するクラス。
    /// </summary>
    public class BattlerKnockbackStateKnockback : BattlerKnockbackState
    {
        private KnockbackInfo Info { get; set; }

        public BattlerKnockbackStateKnockback(BattlerController context, KnockbackInfo info)
            : base(context)
        {
            Info = info;
        }

        public override void Subscribe()
        {
            base.Subscribe();

            // 一定時間で通常の状態に戻る
            Observable.Timer(TimeSpan.FromSeconds(Info.StanTime))
                      .Subscribe(x =>
            {
                ChangeState(new BattlerKnockbackStateNeutral(Context));
            })
                      .AddTo(Disposable);

            // 他のキャラクターにぶつかったらそれも押し飛ばす
            Context.OnCollisionStay2DAsObservable()
                   .Select(x => x.gameObject.GetComponent<EnemyController>())
                   .Where(x => x != null)
                   .Subscribe(x =>
            {
                x.Knockback(Info, Context);
            })
                   .AddTo(Disposable);
        }

        public override void Knockback(KnockbackInfo info, BattlerController against)
        {
        }

        protected override void Control()
        {
        }
    }
}