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
			SetGroundState();
        }

        private void InitializeState()
		{
			if (JumpStateDisposable != null)
			{
				JumpStateDisposable.Dispose();
			}
			JumpStateDisposable = new CompositeDisposable();
		}


        public void SetGroundState()
        {
            InitializeState();
            ActivateJump();
            ActivateWalk();
        }

        public void SetJumpState()
        {
            InitializeState();
            ActivateJump();
            ActivateGroundCheck();
            ActivateGrab();
            ActivateWalk();
        }

        public void SetFullJumpState()
		{
			InitializeState();
            ActivateGroundCheck();
			ActivateGrab();
            ActivateWalk();
		}

        public void SetGrabingWallState(float direction)
        {
            Debug.Log("Grab: " + direction);
            InitializeState();

			Hero.UpdateAsObservable()
				.Where(x => Input.GetKeyDown(KeyCode.Space))
				.Subscribe(x => WallJump(direction))
				.AddTo(JumpStateDisposable);

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

        public void SetWallJumpState()
        {
            InitializeState();
            ActivateGroundCheck();
            ActivateGrab();
            Observable.Timer(TimeSpan.FromMilliseconds(500))
                      .Subscribe(x => CheckJump())
                      .AddTo(JumpStateDisposable);
        }


		private void ActivateJump()
		{
			// スペースキーでジャンプ
			Hero.UpdateAsObservable()
				.Where(x => Input.GetKeyDown(KeyCode.Space))
				.Subscribe(x => Jump())
				.AddTo(JumpStateDisposable);
		}

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

        private void ActivateGrab()
		{
			Hero.OnCollisionStay2DAsObservable()
                .SkipUntil(Observable.Timer(TimeSpan.FromMilliseconds(100)))
				.Where(x => x.gameObject.tag == Def.TerrainTag)
				.Subscribe(x => CheckGrabingWall(x))
				.AddTo(JumpStateDisposable);            
        }

        private void ActivateWalk()
		{
			WalkSubject.Subscribe(x => ActualyWalk())
					   .AddTo(JumpStateDisposable);
        }


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

		private void WallJump(float direction)
		{
            var x = Mathf.Sign(direction) * wallJumpPower;
			Rigidbody.velocity = new Vector2(x, jumpPower);
            Rigidbody.gravityScale = 0;
            Debug.Log(Rigidbody.velocity);

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

        public void Walk()
		{
            WalkSubject.OnNext(Unit.Default);
        }

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