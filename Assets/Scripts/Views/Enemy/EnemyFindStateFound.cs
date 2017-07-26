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
        readonly HeroController hero;

        /// <summary>
        /// EnemyFindStateFound の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="context">この状態を持つ敵キャラクターのビュー。</param>
        /// <param name="hero">敵キャラクターが見つけたプレイヤーのビュー。</param>
        public EnemyFindStateFound(EnemyController context, HeroController hero) : base(context)
        {
            this.hero = hero;
        }

        public override void Control()
        {
            if (hero == null)
            {
                return;
            }

            var direction = (hero.transform.position - Context.transform.position).normalized;
            var velocity = direction * Context.WalkSpeed;
            var body = Context.Rigidbody;
            body.velocity = velocity.MergeY(body.velocity.y);
        }
    }
}