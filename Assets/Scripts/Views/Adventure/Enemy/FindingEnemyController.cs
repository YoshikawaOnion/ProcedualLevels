using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// プレイヤーを発見する前後で異なる振る舞いをする敵キャラクターの振る舞いを提供するコンポーネント。
    /// </summary>
    public abstract class FindingEnemyController : EnemyController
	{
        public EnemyFindState FindState { get; set; }

        public override void Initialize(Models.Enemy enemy, AdventureViewContext context)
        {
			base.Initialize(enemy, context);
			FindState = new EnemyFindStateLookingFor(this);
			FindState.Subscribe();
        }

        public override void Control()
        {
            FindState.Control();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            FindState.Dispose();
            FindState = null;
        }

        /// <summary>
        /// オーバーライドされることで、プレイヤーを発見した後に毎フレーム実行される処理を実行します。
        /// </summary>
        public abstract void ControlAtPlayerFound();
        /// <summary>
        /// オーバーライドされることで、プレイヤーを発見する前に毎フレーム実行される処理を実行します。
        /// </summary>
        public abstract void ControlAtIdle();
    }
}