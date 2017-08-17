using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UniRx.Triggers;
using ProcedualLevels.Models;
using ProcedualLevels.Common;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// キャラクターがノックバックをしている状態の機能を提供するクラス。
    /// </summary>
    public class BattlerKnockbackStateKnockback : BattlerKnockbackState
    {
        private KnockbackInfo Info { get; set; }
        private BattlerController Against { get; set; }

        public BattlerKnockbackStateKnockback(BattlerController context, BattlerController against, KnockbackInfo info)
            : base(context)
        {
            Info = info;
            Against = against;
        }

        /// <summary>
        /// この状態の処理を開始します。
        /// </summary>
        public override void Subscribe()
        {
            base.Subscribe();

            // 一定時間で通常の状態に戻る
            Observable.Timer(TimeSpan.FromSeconds(Info.StanTime * Context.knockbackStanTimeFactor))
                      .Subscribe(x =>
            {
                ChangeState(new BattlerKnockbackStateNeutral(Context));
            })
                      .AddTo(Disposable);

            var enemy = Context.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // 他のキャラクターにぶつかったらそれも押し飛ばす
                Context.OnCollisionStay2DAsObservable()
                       .SkipUntil(Observable.Timer(TimeSpan.FromMilliseconds(64)))
                       .Select(x => x.gameObject.GetComponent<EnemyController>())
                       .Where(x => x != null)
                       .Subscribe(x =>
                {
                    var factor = Helper.GetRandomInRange(0, 0.8f);
                    var power = new Vector2(0, AssetRepository.I.GameParameterAsset.KnockbackJumpPower * factor);
                    var info = Info.Clone();
                    info.StanTime = Info.StanTime + AssetRepository.I.GameParameterAsset.KnockbackChainStanTimeDelta;
                    x.Rigidbody.AddForce(power);
                    x.Knockback(info, Against);
                })
                       .AddTo(Disposable);
            }
        }

        /// <summary>
        /// ノックバックができる状態であればノックバックします。
        /// </summary>
        /// <param name="against">ノックバックを起こした相手のビュー。</param>
        /// <param name="power">ノックバックの強さ。</param>
        public override void Knockback(KnockbackInfo info, BattlerController against)
        {
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
            ChangeState(new BattlerKnockbackStateTrampled(Context));
        }
    }
}