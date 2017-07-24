﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ProcedualLevels.Common;
using UniRx;
using System;
using System.Linq;

namespace ProcedualLevels.Views
{
    public class Player : MonoBehaviour
    {
        public static readonly string GroundStateName = "PlayerStateGround";
        public static readonly string JumpingStateName = "PlayerStateJumping";
        public static readonly string AttackAnimationName = "Attack";
        public static readonly string IdleAnimationName = "Idle";
        public static readonly string DamageAnimationName = "Damage";
        public static readonly string HealthyStateName = "PlayerStateHealthy";
        public static readonly string DamageStateName = "PlayerStateDamaged";

        [SerializeField]
        private float walkSpeed = 2;
        [SerializeField]
        public float jumpPower = 3;
        [SerializeField]
        private int jumpCopyCount = 5;
        [SerializeField]
        private float copyOffset = -8;
        [SerializeField]
        private float jetSpeed = 10;
        [SerializeField]
        private float jetAngleMax = 60;
        [SerializeField]
        public int chargeMax = 8;

        public ReactiveProperty<bool>[] Charges;

        private Script_SpriteStudio_Root sprite;
        private CopyManager copyManager;
        private new Rigidbody2D rigidbody;
        private GameObject copyPrefab;
        public StateMachine JumpState { get; private set; }
        public StateMachine DamageState { get; private set; }

        private IPlayerEventAccepter EventAccepter { get; set; }

        public void Initialize(IPlayerEventAccepter eventAccepter,
                              IGameEventReceiver eventReceiver)
		{
			rigidbody = GetComponent<Rigidbody2D>();
			copyPrefab = Resources.Load<GameObject>("Prefabs/Character/Copy");
			var copyManagerPrefab = Resources.Load<GameObject>("Prefabs/Manager/CopyManager");
			var obj = Instantiate(copyManagerPrefab);
			copyManager = obj.GetComponent<CopyManager>();
			sprite = GetComponentInChildren<Script_SpriteStudio_Root>();
            EventAccepter = eventAccepter;

            var context = new PlayerContext
            {
                Owner = this,
                GameEvents = eventReceiver
            };

            var states = GetComponents<StateMachine>();
            JumpState = states[0];
            //DamageState = states[1];
            ChangeJumpState(JumpingStateName, context);
            //ChangeDamageState(HealthyStateName, context);

            var chargePrefab = Resources.Load<Charge>("Prefabs/Character/Charge");
            Charges = new ReactiveProperty<bool>[chargeMax];
            for (int i = 0; i < chargeMax; i++)
            {
                Charges[i] = new ReactiveProperty<bool>(true);
                var chargeObj = Instantiate(chargePrefab);
                chargeObj.Initialize(this, i);
                chargeObj.transform.SetParent(transform);
            }
        }

        private void OnDestroy()
        {
            copyPrefab = null;
        }

        private void Update()
        {
            transform.rotation = Quaternion.identity;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var prop = Charges.FirstOrDefault(x => x.Value);
                if (prop != null)
				{
                    prop.Value = false;
					StartCoroutine(ShotCoroutine());
                }
            }
        }

        private IEnumerator ShotCoroutine()
		{
			ChangeAnimation(AttackAnimationName);

            yield return new WaitForSeconds(0.1f);

            for (int i = 0; i < jumpCopyCount; i++)
            {
                var angle = Common.Helper.RandomInRange(-jetAngleMax, jetAngleMax) + 180;
                copyManager.CreateCopy(
                    transform.position.AddY(copyOffset).MergeZ(-4),
                    Vector2Extensions.FromAngleLength(angle, jetSpeed));
                yield return new WaitForSeconds(0.15f);
            }

            ChangeAnimation(IdleAnimationName);
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
            rigidbody.velocity = rigidbody.velocity.MergeX(velocity);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                var copy = collision.gameObject.GetComponent<Copy>();
                if (copy != null)
                {
                    copy.Reset();
                }
            }

			if (collision.gameObject.tag == Def.TerrainTag)
			{
				EventAccepter.OnPlayerCollideWithTerrainSender
							 .OnNext(collision);
			}
			if (collision.gameObject.tag == Def.EggTag)
			{
				EventAccepter.OnPlayerCollideWithEggSender
							 .OnNext(collision);
			}
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
			if (collision.gameObject.tag == Def.EnemyTag)
			{
				EventAccepter.OnPlayerCollideWithEnemySender
							 .OnNext(collision);
                ChangeAnimation(DamageAnimationName);
				Observable.Timer(TimeSpan.FromMilliseconds(800))
				  .Subscribe(x => ChangeAnimation(IdleAnimationName));
			}
        }

        public void ChangeJumpState(string stateName, PlayerContext context)
        {
            JumpState.ChangeSubState(stateName, context);
        }

        public void ChangeDamageState(string stateName, PlayerContext context)
        {
            DamageState.ChangeSubState(stateName, context);
        }

        public void ChangeAnimation(string animationName)
        {
            var index = sprite.IndexGetAnimation(animationName);
            sprite.AnimationPlay(index);
        }
    }
}