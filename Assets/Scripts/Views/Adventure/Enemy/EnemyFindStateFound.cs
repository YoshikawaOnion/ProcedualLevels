using System;
using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UnityEngine;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// 敵キャラクターがプレイヤーを見つけた状態の機能を提供します。
    /// </summary>
    public class EnemyFindStateFound : EnemyFindState
    {
        /// <summary>
        /// EnemyFindStateFound の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="context">この状態を持つ敵キャラクターのビュー。</param>
        /// <param name="hero">敵キャラクターが見つけたプレイヤーのビュー。</param>
        public EnemyFindStateFound(FindingEnemyController context) : base(context)
        {
        }

        /// <summary>
        /// 移動などの行動が可能なら行動します。
        /// </summary>
        public override void Control()
        {
            Context.ControlAtPlayerFound();
        }
    }
}