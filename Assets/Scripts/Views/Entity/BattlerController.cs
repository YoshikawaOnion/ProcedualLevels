using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using ProcedualLevels.Models;

namespace ProcedualLevels.Views
{
    public abstract class BattlerController : MonoBehaviour
    {
        public Models.Battler Battler { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }
        public BattlerKnockbackState KnockbackState { get; set; }
        private CompositeDisposable Disposable { get; set; }

        public void Initialize(Models.Battler battler)
		{
            this.Battler = battler;
            Disposable = new CompositeDisposable();

            var canvasPrefab = Resources.Load<GameObject>("Prefabs/Character/BattlerCanvas");
            var canvas = Instantiate(canvasPrefab);
			canvas.transform.SetParent(transform);
			canvas.transform.localPosition = Vector3.zero;

            var hpBar = canvas.transform.Find("HpBar").GetComponent<Image>();
            battler.Hp.Subscribe(x =>
            {
                var ratio = (float)x / battler.MaxHp.Value;
                hpBar.fillAmount = ratio;
            }).AddTo(Disposable);

            Rigidbody = GetComponent<Rigidbody2D>();
            KnockbackState = new BattlerKnockbackStateNeutral(this);
            KnockbackState.Subscribe();
        }

        public void Knockback(KnockbackInfo info, BattlerController against)
        {
            KnockbackState.Knockback(info, against);
        }

        public abstract void Control();
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