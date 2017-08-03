using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public class WalkController : MonoBehaviour
    {
        [SerializeField]
		private float minSpeed;
        [SerializeField]
		private float maxSpeed;
		[SerializeField]
		private float stoppingAccel;
		[SerializeField]
        private float movingAccel;

        private Rigidbody2D Rigidbody { get; set; }

        private void Start()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        private float DiminishSpeed(float powerScale)
        {
            var velocity = Rigidbody.velocity.x;
			var a = -Mathf.Sign(velocity) * stoppingAccel * powerScale;
			velocity = velocity + a;
			if (Mathf.Abs(velocity) <= stoppingAccel)
			{
				velocity = 0;
			}

            return velocity;
        }

        public void Walk(int direction, float powerScale)
        {
			var velocity = Rigidbody.velocity.x;

			if (Mathf.Abs(velocity) < minSpeed)
			{
				// 静止中は初速で歩き始める
				if (direction != 0)
				{
					var v = Mathf.Sign(direction) * minSpeed;
					velocity = v;
				}
                else
				{
                    velocity = DiminishSpeed(powerScale);
                }
            }
			else
			{
				// 静止操作をしている
				if (direction * velocity < 0)
				{
					var a = Mathf.Sign(direction) * stoppingAccel * powerScale;
					velocity = velocity + a;
					if (Mathf.Abs(velocity) <= stoppingAccel)
					{
						velocity = 0;
					}
				}
				else if (direction == 0)
				{
                    velocity = DiminishSpeed(powerScale);
				}
				// 加速操作をしている
				else if (Mathf.Abs(velocity) < maxSpeed)
				{
					var a = Mathf.Sign(direction) * movingAccel * powerScale;
					velocity = velocity + a;
				}
			}
			Rigidbody.velocity = Rigidbody.velocity.MergeX(velocity);
        }
    }
}