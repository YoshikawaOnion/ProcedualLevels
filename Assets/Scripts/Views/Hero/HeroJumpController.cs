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
        [Tooltip("地形との接触時に法線のX要素の絶対値がどれだけの値以下なら着地とみなすか")]
        [SerializeField]
        private float groundNormalXRange;
        [SerializeField]
        private int maxJumpCount;

        private CompositeDisposable JumpStateDisposable { get; set; }
        private HeroController Hero { get; set; }
        private Rigidbody2D Rigidbody { get; set; }
        private int JumpCount { get; set; }
        private float GravityScale { get; set; }

        private void Start()
        {
			Hero = GetComponent<HeroController>();
			Rigidbody = GetComponent<Rigidbody2D>();
            JumpCount = 0;
            GravityScale = Rigidbody.gravityScale;
			SetGroundState();
        }

        private void InitializeJumpState()
		{
			if (JumpStateDisposable != null)
			{
				JumpStateDisposable.Dispose();
			}
			JumpStateDisposable = new CompositeDisposable();
		}

        public void SetGroundState()
		{
			InitializeJumpState();

			Hero.UpdateAsObservable()
				.Where(x => Input.GetKeyDown(KeyCode.Space))
				.Subscribe(x => Jump())
		        .AddTo(JumpStateDisposable);
		}

		public void SetJumpState()
		{
			InitializeJumpState();

            Hero.OnCollisionStay2DAsObservable()
                .SkipUntil(Observable.Timer(TimeSpan.FromMilliseconds(100)))
                .Where(x => x.gameObject.tag == Def.TerrainTag)
			    .Subscribe(collision => CheckGround(collision))
			    .AddTo(JumpStateDisposable);
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

        private void Jump()
        {
            Rigidbody.velocity = Rigidbody.velocity.MergeY(jumpPower);
            Rigidbody.gravityScale = 0;

            var jumpStopper1 = this.UpdateAsObservable()
                                   .SkipWhile(x => !Input.GetKeyUp(KeyCode.Space));
            var jumpStopper2 = this.UpdateAsObservable()
                                   .SkipUntil(Observable.Timer(TimeSpan.FromMilliseconds(250)));
            jumpStopper1.Merge(jumpStopper2)
                        .FirstOrDefault()
                        .Subscribe(x => Rigidbody.gravityScale = GravityScale);

            ++JumpCount;
            if (JumpCount >= maxJumpCount)
            {
				SetJumpState();
			}
        }
    }
}