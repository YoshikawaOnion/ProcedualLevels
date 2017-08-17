using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using ProcedualLevels.Models;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// 戦闘に参加できるキャラクターを制御するクラス。
    /// </summary>
    public abstract class BattlerController : MonoBehaviour
    {
        [SerializeField]
        public float knockbackStanTimeFactor;

        public Models.Battler Battler { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }
        public BattlerKnockbackState KnockbackState { get; set; }
        private CompositeDisposable Disposable { get; set; }

        public void Initialize(Models.Battler battler)
		{
            this.Battler = battler;
            Disposable = new CompositeDisposable();

            var canvasPrefab = Resources.Load<BattlerCanvas>("Prefabs/Character/BattlerCanvas");
            var canvas = Instantiate(canvasPrefab);
			canvas.transform.SetParent(transform);
			canvas.transform.localPosition = Vector3.zero;

            battler.Hp.Subscribe(x =>
            {
                var ratio = (float)x / battler.MaxHp.Value;
                canvas.HpBar.fillAmount = ratio;
            }).AddTo(Disposable);

            Rigidbody = GetComponent<Rigidbody2D>();
            KnockbackState = new BattlerKnockbackStateNeutral(this);
            KnockbackState.Subscribe();
        }

        /// <summary>
        /// このキャラクターをノックバックさせます。
        /// </summary>
        /// <param name="info">ノックバック情報。</param>
        /// <param name="against">ノックバックの相手を表すコンポーネント。</param>
        public void Knockback(KnockbackInfo info, BattlerController against)
        {
            KnockbackState.Knockback(info, against);
        }

        /// <summary>
        /// 毎フレーム行う処理を実行します。
        /// </summary>
        public abstract void Control();
        /// <summary>
        /// キャラクターが死亡した際の処理を行います。
        /// </summary>
        public abstract void Die();

        protected virtual void OnDestroy()
        {
            Disposable.Dispose();
            if (KnockbackState != null)
			{
				KnockbackState.Dispose();
				KnockbackState = null;
            }
        }
    }
}