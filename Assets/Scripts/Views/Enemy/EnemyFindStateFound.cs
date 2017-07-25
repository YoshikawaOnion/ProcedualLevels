using System;
using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public class EnemyFindStateFound : EnemyFindState
    {
        readonly HeroController hero;

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