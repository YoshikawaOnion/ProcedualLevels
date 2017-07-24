using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProcedualLevels.Models;
using ProcedualLevels.Common;
using UniRx;
using UniRx.Triggers;

namespace ProcedualLevels.Views
{
    public class HeroController : MonoBehaviour
    {
        [SerializeField]
        private float walkSpeed;

        public Hero Hero { get; private set; }
        private Rigidbody2D Rigidbody { get; set; }
        private IPlayerEventAccepter EventAccepter { get; set; }

        public void Initialize(Hero hero, IPlayerEventAccepter eventAccepter)
        {
            Hero = hero;
            EventAccepter = eventAccepter;

            var battler = GetComponent<BattlerController>();
            battler.Initialize(hero);
        }

        // Use this for initialization
        void Start()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
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
    }
}