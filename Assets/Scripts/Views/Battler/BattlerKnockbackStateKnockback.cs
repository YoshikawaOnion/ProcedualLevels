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
                    info.StanTime = Info.StanTime + 0.1f;
                    x.Rigidbody.AddForce(power);
                    x.Knockback(info, Against);
                })
                       .AddTo(Disposable);
            }
        }

        public override void Knockback(KnockbackInfo info, BattlerController against)
        {
        }

        protected override void Control()
        {
        }
    }
}