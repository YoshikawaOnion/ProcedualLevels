using System;
using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// 無制限にプレイヤーを追い続ける敵キャラクターの振る舞いを提供します。
    /// </summary>
    public class UnlimitedChasingEnemyController : EnemyController
    {
        private MoveController MoveController { get; set; }
        private bool IsStaying { get; set; }
        private CompositeDisposable Disposable { get; set; }

        public override void Initialize(Models.Enemy enemy, AdventureViewContext context)
        {
			base.Initialize(enemy, context);
			MoveController = GetComponent<MoveController>();
            Disposable = new CompositeDisposable();

            var stayTriggerTimes = (int)Helper.GetRandomInRange(20, (float)40);
            Observable.Interval(TimeSpan.FromSeconds(stayTriggerTimes))
                .FirstOrDefault()
                .Subscribe(x =>
            {
                IsStaying = true;
                Rigidbody.velocity = Vector2.zero;
                Observable.Timer(TimeSpan.FromSeconds(2))
                          .Subscribe(y => IsStaying = false);
            })
                      .AddTo(Disposable);
        }

        /// <summary>
        /// 毎フレーム行う処理を実行します。
        /// </summary>
        public override void Control()
		{
			if (Context.Hero == null || IsStaying)
			{
                Rigidbody.velocity = Vector2.zero;
				return;
			}

			var direction = (Context.Hero.transform.position - transform.position).normalized;
			var walkDirectionX = Helper.Sign(direction.x, 0.2f);
            var walkDirectionY = Helper.Sign(direction.y, 0.2f);
			var x = MoveController.GetMoveSpeed(Rigidbody.velocity.x, walkDirectionX, 1);
			var y = MoveController.GetMoveSpeed(Rigidbody.velocity.y, walkDirectionY, 1);
			Rigidbody.velocity = new Vector2(x, y);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Disposable.Dispose();
        }
    }
}