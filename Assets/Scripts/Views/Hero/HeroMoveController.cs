using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using System;
using ProcedualLevels.Common;

namespace ProcedualLevels.Views
{
    public class HeroMoveController : MonoBehaviour
    {
        [SerializeField]
        private float jumpPower;
        [SerializeField]
        private float maxJumpTime;
        [SerializeField]
        private float wallJumpPower;
        [Tooltip("地形との接触時に法線のX要素の絶対値がどれだけの値以下なら着地とみなすか")]
        [SerializeField]
        private float groundNormalXRange;
        [SerializeField]
		private int maxJumpCount;
		[SerializeField]
		private float walkSpeed;

        private CompositeDisposable JumpStateDisposable { get; set; }
        private HeroController Hero { get; set; }
        private Rigidbody2D Rigidbody { get; set; }
        private HeroAnimationController Animation { get; set; }

        private float GravityScale { get; set; }
        private int JumpCount { get; set; }
        private int WalkDirection { get; set; }
        private Subject<int> WalkSubject { get; set; }
        private Subject<bool> JumpSubject { get; set; }

        private void Start()
        {
			Hero = GetComponent<HeroController>();
			Rigidbody = GetComponent<Rigidbody2D>();
            Animation = GetComponent<HeroAnimationController>();
            WalkSubject = new Subject<int>();
            JumpSubject = new Subject<bool>();
            JumpCount = 0;
            GravityScale = Rigidbody.gravityScale;
			SetFullJumpState();
            WalkDirection = 0;
        }

        /// <summary>
        /// 現在の状態に紐づけられた振る舞いを停止します。
        /// </summary>
        private void InitializeState()
		{
			if (JumpStateDisposable != null)
			{
				JumpStateDisposable.Dispose();
			}
			JumpStateDisposable = new CompositeDisposable();
		}

        /// <summary>
        /// プレイヤーが地面に接している状態に遷移します。
        /// </summary>
        public void SetGroundState()
        {
            InitializeState();
            ActivateJump();
            ActivateWalk();
            ActivateFallCheck();
        }

        /// <summary>
        /// プレイヤーがジャンプをしていて、かつ空中ジャンプが可能な状態に遷移します。
        /// </summary>
        public void SetJumpState()
        {
            InitializeState();
            ActivateJump();
            ActivateGroundCheck();
            ActivateGrab();
            ActivateWalk();
        }

        /// <summary>
        /// プレイヤーが空中ジャンプを使い果たした状態に遷移します。
        /// </summary>
        public void SetFullJumpState()
		{
			InitializeState();
            ActivateGroundCheck();
			ActivateGrab();
            ActivateWalk();
		}

        /// <summary>
        /// プレイヤーが壁に張り付いている状態に遷移します。
        /// </summary>
        /// <param name="direction">壁との接点の法線X方向。</param>
        public void SetGrabingWallState(float direction)
        {
            InitializeState();

            // 一度ジャンプキーを離してから再びジャンプすると壁ジャンプ
            JumpSubject.SkipWhile(x => x)
                       .Where(x => x)
                       .FirstOrDefault()
                       .Subscribe(x => WallJump(direction))
                       .AddTo(JumpStateDisposable);
            
            // 方向キーを離すとジャンプ状態に遷移
            WalkSubject.SkipWhile(x => x * direction <= 0)
                       .FirstOrDefault()
                       .Subscribe(x => 
            {
                Rigidbody.gravityScale = GravityScale;
                Rigidbody.velocity = new Vector2(0, -1);
                CheckJump();
            })
                       .AddTo(JumpStateDisposable);
		}

        /// <summary>
        /// プレイヤーが壁蹴りジャンプをしている状態に遷移します。
        /// </summary>
        public void SetWallJumpState()
        {
            InitializeState();
            ActivateGroundCheck();
            ActivateGrab();

            // 時間が経過するとジャンプ状態に遷移
            Observable.Timer(TimeSpan.FromMilliseconds(500))
                      .Subscribe(x => CheckJump())
                      .AddTo(JumpStateDisposable);
        }


        /// <summary>
        /// 現在の状態でジャンプができるようにします。
        /// </summary>
		private void ActivateJump()
		{
            JumpSubject.SkipWhile(x => x)
                       .Where(x => x)
                       .FirstOrDefault()
                       .Repeat()
                       .Subscribe(x => Jump())
                       .AddTo(JumpStateDisposable);
		}

        /// <summary>
        /// 現在の状態で着地判定をするようにします。
        /// </summary>
		private void ActivateGroundCheck()
		{
            // 地面に着地したら GroundState へ(ジャンプの瞬間に着地判定をしないように)
            Hero.OnCollisionStay2DAsObservable()
                .SkipUntil(Observable.Timer(TimeSpan.FromMilliseconds(100)))
                .Where(x => x.gameObject.tag == Def.TerrainTag
                       || x.gameObject.tag == Def.PlatformTag)
				.Subscribe(collision => CheckGround(collision))
				.AddTo(JumpStateDisposable);
		}

        /// <summary>
        /// 現在の状態で壁に張り付けるようにします。
        /// </summary>
        private void ActivateGrab()
		{
			Hero.OnCollisionStay2DAsObservable()
                .SkipUntil(Observable.Timer(TimeSpan.FromMilliseconds(100)))
				.Where(x => x.gameObject.tag == Def.TerrainTag
                      || x.gameObject.tag == Def.PlatformTag)
				.Subscribe(x => CheckGrabingWall(x))
				.AddTo(JumpStateDisposable);            
        }

        /// <summary>
        /// 現在の状態で足場がないと落下状態にするようにします。
        /// </summary>
        private void ActivateFallCheck()
        {
            Hero.OnCollisionExit2DAsObservable()
                .Where(x => x.gameObject.tag == Def.TerrainTag
                       || x.gameObject.tag == Def.PlatformTag)
                .Subscribe(x =>
            {
                CheckJump();
                ++JumpCount;
            })
                .AddTo(JumpStateDisposable);
        }

        /// <summary>
        /// 現在の状態で歩行できるようにします。
        /// </summary>
        private void ActivateWalk()
		{
            WalkSubject.Subscribe(x => ActualyWalk())
                       .AddTo(JumpStateDisposable);
        }


        /// <summary>
        /// 壁に張り付く状況であれば壁に張り付いた状態に遷移します。
        /// </summary>
        /// <param name="collision">Collision.</param>
        private void CheckGrabingWall(Collision2D collision)
        {
            var contact = collision.contacts[0];
            if (contact.normal.y <= groundNormalXRange
               && contact.normal.y >= -groundNormalXRange)
            {
                SetGrabingWallState(contact.normal.x);
				Rigidbody.gravityScale = GravityScale / 2;
				Rigidbody.velocity = Vector3.zero;
            }
        }

        /// <summary>
        /// 着地すべき状況であれば着地状態に遷移します。
        /// </summary>
        /// <param name="collision">Collision.</param>
		private void CheckGround(Collision2D collision)
		{
			var contact = collision.contacts[0];
            if (contact.normal.x <= groundNormalXRange
				&& contact.normal.x >= -groundNormalXRange
				&& contact.normal.y > 0)
			{
				SetGroundState();
                JumpCount = 0;
			}
		}

        /// <summary>
        /// 空中ジャンプができるかどうかに応じて、どちらかのジャンプ状態に遷移します。
        /// </summary>
        private void CheckJump()
        {
            if (JumpCount >= maxJumpCount)
            {
                SetFullJumpState();
            }
            else
            {
                SetJumpState();
            }
        }


        /// <summary>
        /// このプレイヤーにジャンプをさせます。
        /// </summary>
        private void Jump()
        {
            Rigidbody.velocity = Rigidbody.velocity.MergeY(jumpPower);
            Rigidbody.gravityScale = 0;

            var jumpStopper1 = JumpSubject.SkipWhile(x => x)
                                          .Select(x => Unit.Default);
            var jumpStopper2 = Observable.Timer(TimeSpan.FromSeconds(maxJumpTime))
                                         .Select(x => Unit.Default);
            jumpStopper1.Merge(jumpStopper2)
                        .FirstOrDefault()
                        .Subscribe(x => Rigidbody.gravityScale = GravityScale);

            ++JumpCount;
            CheckJump();
        }

        /// <summary>
        /// このプレイヤーに壁蹴りジャンプをさせます。
        /// </summary>
        /// <param name="direction">壁蹴りジャンプの方向。</param>
		private void WallJump(float direction)
		{
            var x = Mathf.Sign(direction) * wallJumpPower;
			Rigidbody.velocity = new Vector2(x, jumpPower);
            Rigidbody.gravityScale = 0;

			var jumpStopper1 = JumpSubject.SkipWhile(t => t)
										  .Select(t => Unit.Default);
			var jumpStopper2 = Observable.Timer(TimeSpan.FromSeconds(maxJumpTime))
										 .Select(t => Unit.Default);
			jumpStopper1.Merge(jumpStopper2)
						.FirstOrDefault()
						.Subscribe(t => Rigidbody.gravityScale = GravityScale);

			Animation.AnimateWalk(direction);
            SetWallJumpState();
        }

        /// <summary>
        /// このプレイヤーの歩行状態を更新します。
        /// </summary>
        public void ControlWalk(int direction)
		{
            WalkSubject.OnNext(direction);
            WalkDirection = direction;
        }

        /// <summary>
        /// このプレイヤーのジャンプ状態を更新します。
        /// </summary>
        public void ControlJump(bool isJumping)
        {
            JumpSubject.OnNext(isJumping);
        }

        /// <summary>
        /// このプレイヤーに歩かせます。
        /// </summary>
        private void ActualyWalk()
		{
            float velocity = Mathf.Sign(WalkDirection) * walkSpeed;
            if (WalkDirection == 0)
            {
                velocity = 0;
            }

			Rigidbody.velocity = Rigidbody.velocity.MergeX(velocity);
			Animation.AnimateWalk(velocity);            
        }
    }
}