using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;
using System;
using ProcedualLevels.Common;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// 敵キャラクターの振る舞いを提供するクラス。
    /// </summary>
    public abstract class EnemyController : BattlerController
    {
        public Models.Enemy Enemy { get; private set; }
        protected EnemyAnimationController Animation { get; set; }
        protected AdventureViewContext Context { get; set; }

        public virtual void Initialize(Models.Enemy enemy, AdventureViewContext context)
        {
            base.Initialize(enemy);
            Enemy = enemy;
            Context = context;

            Animation = GetComponent<EnemyAnimationController>();
            Animation.Initialize(context.EventReceiver);
        }

        /// <summary>
        /// 毎フレーム行う処理を実行します。
        /// </summary>
        public override abstract void Control();

        /// <summary>
        /// キャラクターが死亡した際の処理を行います。
        /// </summary>
        public override void Die()
        {
            Animation.AnimateDie()
                     .Subscribe(x =>
            {
                Destroy(gameObject);
            });
        }
    }   
}
