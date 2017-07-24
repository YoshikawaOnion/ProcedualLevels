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

        private HeroContext Context { get; set; }
        private CompositeDisposable JumpStateDisposable { get; set; }

        public void Initialize(HeroContext context)
        {
            Context = context;
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

			Context.Hero.UpdateAsObservable()
				   .Where(x => Input.GetKeyDown(KeyCode.Space))
				   .Subscribe(x => Jump())
				   .AddTo(JumpStateDisposable);
		}

		private void SetJumpState()
		{
			InitializeJumpState();
			
			Context.GameEvents.OnPlayerCollideWithTerrainReceiver
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
            var rigidbody = Context.Hero.GetComponent<Rigidbody2D>();
            rigidbody.AddForce(new Vector2(0, jumpPower));
            SetJumpState();
        }
    }
}