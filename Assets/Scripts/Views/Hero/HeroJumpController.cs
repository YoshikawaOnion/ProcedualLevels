using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

namespace ProcedualLevels.Views
{
    public class HeroJumpController : MonoBehaviour
    {
        [SerializeField]
        private float jumpPower;

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

            Hero.OnCollisionEnter2DAsObservable()
                   .Where(x => x.gameObject.tag == Def.TerrainTag)
			       .Subscribe(collision => CheckGround(collision))
			       .AddTo(JumpStateDisposable);
		}

		private void CheckGround(Collision2D collision)
		{
			var contact = collision.contacts[0];
			if (contact.normal.x <= 0.15f
				&& contact.normal.x >= -0.15f
				&& contact.normal.y > 0)
			{
				SetGroundState();
			}
		}

        private void Jump()
        {
            Rigidbody.AddForce(new Vector2(0, jumpPower));
            SetJumpState();
        }
    }
}