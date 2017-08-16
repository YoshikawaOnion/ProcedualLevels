using System;
using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using ProcedualLevels.Models;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// キャラクターのノックバックに関わる状態の基底クラス。
    /// </summary>
    public class BattlerKnockbackStateTrampled : BattlerKnockbackStateNeutral
    {
        public BattlerKnockbackStateTrampled(BattlerController context) : base(context)
        {
        }

        public override void Subscribe()
        {
            base.Subscribe();

            Context.Rigidbody.velocity = Vector2.zero;

            var enemy = Context.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // 他のキャラクターにぶつかったらそれも押し飛ばす
                Context.OnCollisionStay2DAsObservable()
                       .Select(x => x.gameObject.GetComponent<EnemyController>())
                       .Where(x => x != null)
                       .Subscribe(x => x.KnockbackState.OnTrampled())
                       .AddTo(Disposable);
            }

            var recoverTime = AssetRepository.I.GameParameterAsset.RecoverTimeFromTrampled;

            Context.UpdateAsObservable()
                   .SkipUntil(Observable.Timer(TimeSpan.FromSeconds(recoverTime)))
                   .FirstOrDefault()
                   .Subscribe(x => Context.KnockbackState.ChangeState(new BattlerKnockbackStateNeutral(Context)))
                   .AddTo(Disposable);
        }

        protected override void Control()
        {
        }

        public override void OnTrampled()
        {
        }
    }
}