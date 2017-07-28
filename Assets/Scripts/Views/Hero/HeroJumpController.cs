using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using System;
using ProcedualLevels.Common;

namespace ProcedualLevels.Views
{
    public class HeroJumpController : MonoBehaviour
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
        private int JumpCount { get; set; }
        private float GravityScale { get; set; }
        private HeroAnimationController Animation { get; set; }
        private Subject<Unit> WalkSubject { get; set; }

        private void Start()
        {
			Hero = GetComponent<HeroController>();
			Rigidbody = GetComponent<Rigidbody2D>();
            Animation = GetComponent<HeroAnimationController>();
            WalkSubject = new Subject<Unit>();
            JumpCount = 0;
            GravityScale = Rigidbody.gravityScale;
			SetFullJumpState();
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

            // ジャンプキーで壁蹴りジャンプ
			Hero.UpdateAsObservable()
				.Where(x => Input.GetKeyDown(KeyCode.Space))
				.Subscribe(x => WallJump(direction))
				.AddTo(JumpStateDisposable);

            // 方向キーを離すとジャンプ状態に遷移
            Hero.UpdateAsObservable()
                .SkipWhile(x => Input.GetAxis("Horizontal") * direction < 0)
                .FirstOrDefault()
                .Subscribe(x => 
            {
                Rigidbody.gravityScale = GravityScale;
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
			// スペースキーでジャンプ
			Hero.UpdateAsObservable()
				.Where(x => Input.GetKeyDown(KeyCode.Space))
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
				.Where(x => x.gameObject.tag == Def.TerrainTag)
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
               && contact.normal.y >= -groundNormalXRange
               && Input.GetAxis("Horizontal") * contact.normal.x < 0)
            {
                SetGrabingWallState(contact.normal.x);
				Rigidbody.gravityScale = 0;
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

            var jumpStopper1 = this.UpdateAsObservable()
                                   .SkipWhile(x => !Input.GetKeyUp(KeyCode.Space));
            var jumpStopper2 = this.UpdateAsObservable()
                                   .SkipUntil(Observable.Timer(TimeSpan.FromSeconds(maxJumpTime)));
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

			var jumpStopper1 = this.UpdateAsObservable()
								   .SkipWhile(t => !Input.GetKeyUp(KeyCode.Space));
			var jumpStopper2 = this.UpdateAsObservable()
								   .SkipUntil(Observable.Timer(TimeSpan.FromSeconds(maxJumpTime)));
			jumpStopper1.Merge(jumpStopper2)
						.FirstOrDefault()
						.Subscribe(t => Rigidbody.gravityScale = GravityScale);

			Animation.AnimateWalk(direction);
            SetWallJumpState();
        }

        /// <summary>
        /// このプレイヤーが歩ける状況であれば歩かせます。
        /// </summary>
        public void Walk()
		{
            WalkSubject.OnNext(Unit.Default);
        }

        /// <summary>
        /// このプレイヤーに歩かせます。
        /// </summary>
        private void ActualyWalk()
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

			Animation.AnimateWalk(velocity);            
        }
    }
}