using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// キャラクターの歩行を制御するコンポーネント。
    /// </summary>
    public class MoveController : MonoBehaviour
	{
		[SerializeField]
		private float minSpeed = 3;
		[SerializeField]
		private float maxSpeed = 5;
        [SerializeField]
        private float stoppingAccel = 0.5f;
        [SerializeField]
        private float movingAccel = 0.1f;

		private Rigidbody2D Rigidbody { get; set; }

		private void Start()
		{
			Rigidbody = GetComponent<Rigidbody2D>();
		}

		private float DiminishSpeed(float currentVelocity, float powerScale)
		{
			var velocity = currentVelocity;
			var a = -Mathf.Sign(velocity) * stoppingAccel * powerScale;
			velocity += a;
			if (Mathf.Abs(velocity) <= stoppingAccel)
			{
				velocity = 0;
			}

			return velocity;
		}

        /// <summary>
        /// 現在の状態を加味した新しい移動速度を取得します。
        /// </summary>
        /// <returns>新しい移動速度。</returns>
        /// <param name="currentVelocity">現在の移動速度。</param>
        /// <param name="direction">新しい移動の向き。</param>
        /// <param name="powerScale">移動量にかける倍率。</param>
        public float GetMoveSpeed(float currentVelocity, int direction, float powerScale)
		{
			var velocity = currentVelocity;

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
					velocity = DiminishSpeed(velocity, powerScale);
				}
			}
			else
			{
				// 静止操作をしている
				if (direction * velocity < 0)
				{
					var a = Mathf.Sign(direction) * stoppingAccel * powerScale;
					velocity += a;
					if (Mathf.Abs(velocity) <= stoppingAccel)
					{
						velocity = 0;
					}
				}
				else if (direction == 0)
				{
					velocity = DiminishSpeed(velocity, powerScale);
				}
				// 加速操作をしている
				else if (Mathf.Abs(velocity) < maxSpeed)
				{
					var a = Mathf.Sign(direction) * movingAccel * powerScale;
					velocity += a;
				}
			}

            return velocity;
		}
    }
}