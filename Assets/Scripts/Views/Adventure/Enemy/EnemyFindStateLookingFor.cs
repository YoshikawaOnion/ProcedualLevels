using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// 敵キャラクターがプレイヤーをまだ発見していない状態の機能を提供するクラス。
    /// </summary>
    public class EnemyFindStateLookingFor : EnemyFindState
    {
        /// <summary>
        /// EnemyFindStateLookingFor の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="context">この状態を持つ敵キャラクターのビュー。</param>
        public EnemyFindStateLookingFor(FindingEnemyController context) : base(context)
        {
        }

        /// <summary>
        /// この状態の処理を開始します。
        /// </summary>
        public override void Subscribe()
        {
            base.Subscribe();
            var searchArea = Context.SearchArea;
            searchArea.OnTriggerEnter2DAsObservable()
                      .Subscribe(x => OnSearch(x))
                      .AddTo(Disposable);
        }

        /// <summary>
        /// 見つけたオブジェクトがプレイヤーならば、発見した状態に遷移します。
        /// </summary>
        /// <param name="collider">捜索範囲に入ったコライダー。</param>
        private void OnSearch(Collider2D collider)
        {
            var hero = collider.GetComponent<HeroController>();
            if (hero != null)
            {
                ChangeState(new EnemyFindStateFound(Context));
            }
        }

        /// <summary>
        /// 移動などの行動が可能なら行動します。
        /// </summary>
        public override void Control()
        {
            Context.ControlAtIdle();
        }
    }
}