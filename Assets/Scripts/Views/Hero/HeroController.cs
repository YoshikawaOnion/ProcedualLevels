using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProcedualLevels.Models;
using ProcedualLevels.Common;
using UniRx;
using UniRx.Triggers;
using System;

namespace ProcedualLevels.Views
{
    public class HeroController : BattlerController
    {
        [SerializeField]
        private float walkSpeed;

        public Hero Hero { get; private set; }
        private IPlayerEventAccepter EventAccepter { get; set; }

        public void Initialize(Hero hero, IPlayerEventAccepter eventAccepter)
        {
            Hero = hero;
            EventAccepter = eventAccepter;
            base.Initialize(hero);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var enemy = collision.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                EventAccepter.OnPlayerBattleWithEnemySender
                             .OnNext(enemy.Enemy);
            }
        }
		
		public override void Control()
		{
			float velocity = 0;
			if (Input.GetKey(KeyCode.RightArrow))
			{
				velocity += walkSpeed;
			}
			if (Input.GetKey(KeyCode.LeftArrow))
			{
				velocity -= walkSpeed;
			}
			Rigidbody.velocity = Rigidbody.velocity.MergeX(velocity);
            Debug.DrawLine(transform.position,
                           transform.position + Rigidbody.velocity.ToVector3() * 10,
                           new Color(255, 0, 0));
		}
    }
}