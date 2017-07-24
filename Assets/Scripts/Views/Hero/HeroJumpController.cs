using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using System;

namespace ProcedualLevels.Views
{
    public class HeroJumpController : MonoBehaviour
    {
        [SerializeField]
        private float jumpPower;
        [Tooltip("地形との接触時に法線のX要素の絶対値がどれだけの値以下なら着地とみなすか")]
        [SerializeField]
        private float groundNormalXRange;

        private CompositeDisposable JumpStateDisposable { get; set; }
        private HeroController Hero { get; set; }
        private Rigidbody2D Rigidbody { get; set; }

        private void Start()
        {
			Hero = GetComponent<HeroController>();
			Rigidbody = GetComponent<Rigidbody2D>();
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

        private void SetGroundState()
		{
			InitializeJumpState();

			Hero.UpdateAsObservable()
				.Where(x => Input.GetKeyDown(KeyCode.Space))
				.Subscribe(x => Jump())
		        .AddTo(JumpStateDisposable);
		}

		private void SetJumpState()
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
			}
		}

        private void Jump()
        {
			SetJumpState();
			Rigidbody.AddForce(new Vector2(0, jumpPower));
        }
    }
}