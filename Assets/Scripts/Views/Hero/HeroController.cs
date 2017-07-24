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

        private Hero Hero { get; set; }
        private Rigidbody2D Rigidbody { get; set; }
        private IPlayerEventAccepter EventAccepter { get; set; }

        public void Initialize(Hero hero,
                               IGameEventReceiver eventReceiver,
                               IPlayerEventAccepter eventAccepter)
        {
            Hero = hero;
            EventAccepter = eventAccepter;

            var jump = GetComponent<HeroJumpController>();
            var context = new HeroContext()
            {
                Hero = this,
                GameEvents = eventReceiver
            };
            jump.Initialize(context);
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
            if (collision.gameObject.tag == Def.TerrainTag)
            {
                EventAccepter.OnPlayerCollideWithTerrainSender
                             .OnNext(collision);
            }
        }
    }
}