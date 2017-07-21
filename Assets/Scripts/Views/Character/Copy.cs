using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace ProcedualLevels.Views
{
    public class Copy : MonoBehaviour
    {
        [SerializeField]
        private Vector2 ballUv;
        [SerializeField]
        private Vector2 dropUv0;
        [SerializeField]
        private Vector2 dropUv1;
        [SerializeField]
        private float VanishTime;
        [SerializeField]
        private float GravityAtFalling;

        public bool IsOnGround { get; private set; }
        public Vector2 Uv { get; private set; }
        public IObservable<Unit> OnVanish { get; private set; }

        private Subject<Unit> OnVanish_ { get; set; }
        private Rigidbody2D Rigidbody { get; set; }

        public void Initialize()
		{
			Observable.Timer(TimeSpan.FromSeconds(VanishTime))
					  .Subscribe(x => Reset());
			OnVanish_ = new Subject<Unit>();
			OnVanish = OnVanish_;
            gameObject.SetActive(true);
			Rigidbody = GetComponent<Rigidbody2D>();

            SetGroundState(false);
        }

        public void Reset()
        {
            gameObject.SetActive(false);
            OnVanish_.OnNext(Unit.Default);
            OnVanish_.OnCompleted();
        }

        private void SetGroundState(bool isOnGround)
        {
            IsOnGround = isOnGround;
            Rigidbody.gravityScale = isOnGround ? 1 : GravityAtFalling;
            if (isOnGround)
			{
				Uv = dropUv1;
                Observable.Timer(TimeSpan.FromMilliseconds(50))
                          .Subscribe(x => Uv = ballUv);
            }
            else
            {
                Uv = dropUv0;
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (IsOnGround)
            {
                return;
            }

            var copy = collision.gameObject.GetComponent<Copy>();
	        if (copy != null)
	        {
                if (!copy.IsOnGround)
                {
                    return;
                }
                SetGroundState(true);
            }

            if (collision.gameObject.tag == Def.TerrainTag)
			{
                var contact = collision.contacts[0];
                if (contact.normal.x <= 0.15f
                   && contact.normal.x >= -0.15f)
				{
                    SetGroundState(true);
                }
            }
        }
    }
}