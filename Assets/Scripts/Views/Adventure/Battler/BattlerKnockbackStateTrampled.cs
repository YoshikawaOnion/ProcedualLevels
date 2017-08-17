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

        /// <summary>
        /// この状態の処理を開始します。
        /// </summary>
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

        /// <summary>
        /// 移動などの行動ができれば行動します。
        /// </summary>
        protected override void Control()
        {
        }

        /// <summary>
        /// 派生クラスで実装されることで、キャラクターがプレイヤーに踏みつけられた時の処理を実行します。
        /// </summary>
        public override void OnTrampled()
        {
        }
    }
}