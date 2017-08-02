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
        private float jumpPower = 0;
        [SerializeField]
        private float maxJumpTime = 0;
        [SerializeField]
        private float wallJumpPower = 0;
        [Tooltip("地形との接触時に法線のX要素の絶対値がどれだけの値以下なら着地とみなすか")]
        [SerializeField]
        private float groundNormalXRange = 0;
        [SerializeField]
        private int maxJumpCount = 0;
        [Tooltip("ジャンプ中の加速度倍率")]
        [SerializeField]
        private float jumpingAccelScale = 0;
        [Tooltip("壁ジャンプ中の加速度倍率")]
        [SerializeField]
        private float wallJumpingAccelScale = 0;
        [Tooltip("壁張り付き中の落下速度")]
        [SerializeField]
        private float wallDraggingVelocity = 0;

        private CompositeDisposable JumpStateDisposable { get; set; }
        private HeroController Hero { get; set; }
        private Rigidbody2D Rigidbody { get; set; }
        private HeroAnimationController Animation { get; set; }
        private Collider2D WallDetecter { get; set; }
        private WalkController WalkController { get; set; }

        private float GravityScale { get; set; }
        private int JumpCount { get; set; }
        private int WalkDirection { get; set; }
        private bool IsOnGround { get; set; }
        private Subject<int> WalkSubject { get; set; }
        private Subject<bool> JumpSubject { get; set; }

        private void Start()
        {
            Hero = GetComponent<HeroController>();
            Rigidbody = GetComponent<Rigidbody2D>();
            Animation = GetComponent<HeroAnimationController>();
            WalkController = GetComponent<WalkController>();
            WalkSubject = new Subject<int>();
            JumpSubject = new Subject<bool>();
            WallDetecter = transform.Find("WallDetecter").GetComponent<Collider2D>();
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
            //Debug.Log("State: Ground");
            InitializeState();
            ActivateJump();
            ActivateWalk(1);
            ActivateFallCheck();
        }

        /// <summary>
        /// プレイヤーがジャンプをしていて、かつ空中ジャンプが可能な状態に遷移します。
        /// </summary>
        public void SetJumpState()
        {
            //Debug.Log("State: Jump");
            InitializeState();
            ActivateJump();
            ActivateGroundCheck();
            ActivateGrab();
            ActivateWalk(jumpingAccelScale);
        }

        /// <summary>
        /// プレイヤーが空中ジャンプを使い果たした状態に遷移します。
        /// </summary>
        public void SetFullJumpState()
        {
            //Debug.Log("State: FullJump");
            InitializeState();
            ActivateGroundCheck();
            ActivateGrab();
            ActivateWalk(jumpingAccelScale);
        }

        /// <summary>
        /// プレイヤーが壁に張り付いている状態に遷移します。
        /// </summary>
        /// <param name="direction">壁との接点の法線X方向。</param>
        public void SetGrabingWallState(float direction)
        {
            //Debug.Log("State: GrabingWall");
            InitializeState();
            ActivateGroundCheck();
            ActivateWalk(jumpingAccelScale);

            // 一度ジャンプキーを離してから再びジャンプすると壁ジャンプ
            //*/
            JumpSubject.SkipWhile(x => x)
                       .Where(x => x)
                       .FirstOrDefault()
                       .Subscribe(x => WallJump(direction))
                       .AddTo(JumpStateDisposable);
            //*/

            // ずり落ちたり壁を離れたらジャンプ状態へ遷移
            WallDetecter.OnTriggerExit2DAsObservable()
                        .Where(x => x.gameObject.tag == Def.TerrainTag
                               || x.gameObject.tag == Def.PlatformTag)
                        .FirstOrDefault()
                        .Subscribe(x => CheckJump())
                        .AddTo(JumpStateDisposable);

            // 順方向のキーを押し込むと落下速度が減少
            WalkSubject.SkipWhile(x => Rigidbody.velocity.y > 0)
                       .Where(x => x * direction < 0)
                       .Subscribe(x => Rigidbody.velocity = Rigidbody.velocity
                                  .MergeY(-wallDraggingVelocity))
                       .AddTo(JumpStateDisposable);
        }

        /// <summary>
        /// プレイヤーが壁蹴りジャンプをしている状態に遷移します。
        /// </summary>
        public void SetWallJumpState()
        {
            //Debug.Log("State: WallJump");
            InitializeState();
            ActivateGroundCheck();
            ActivateGrab();
            ActivateWalk(wallJumpingAccelScale);
            ActivateJump();

            // 時間が経過するとジャンプ状態に遷移
            Observable.Timer(TimeSpan.FromMilliseconds(200))
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
        private void ActivateWalk(float powerScale)
        {
            WalkSubject.Subscribe(x => ActualyWalk(powerScale))
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
            }
        }

        /// <summary>
        /// 着地すべき状況であれば着地状態に遷移します。
        /// </summary>
        /// <param name="collision">Collision.</param>
		private void CheckGround(Collision2D collision)
        {
            foreach (var contact in collision.contacts)
            {
                if (contact.normal.x <= groundNormalXRange
                    && contact.normal.x >= -groundNormalXRange
                    && contact.normal.y > 0)
                {
                    SetGroundState();
                    IsOnGround = true;
                    JumpCount = 0;
                }
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
            IsOnGround = false;
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
            IsOnGround = false;
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
        private void ActualyWalk(float powerScale)
        {
            WalkController.Walk(WalkDirection, powerScale);

            var velocity = Rigidbody.velocity.x;
            Animation.AnimateWalk(Mathf.Abs(velocity) < 1 ? 0 : Mathf.Sign(velocity));
        }
    }
}