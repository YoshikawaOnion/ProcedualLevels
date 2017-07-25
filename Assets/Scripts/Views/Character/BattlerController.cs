using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

namespace ProcedualLevels.Views
{
    public abstract class BattlerController : MonoBehaviour
    {
        public Models.Battler Battler { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }
        public BattlerKnockbackState KnockbackState { get; set; }

        public void Initialize(Models.Battler battler)
		{
            this.Battler = battler;

            var canvasPrefab = Resources.Load<GameObject>("Prefabs/Character/BattlerCanvas");
            var canvas = Instantiate(canvasPrefab);
			canvas.transform.SetParent(transform);
			canvas.transform.localPosition = Vector3.zero;

            var hpBar = canvas.transform.Find("HpBar").GetComponent<Image>();
            battler.Hp.Subscribe(x =>
            {
                var ratio = (float)x / battler.MaxHp.Value;
                hpBar.fillAmount = ratio;
            });

            Rigidbody = GetComponent<Rigidbody2D>();
            KnockbackState = new BattlerKnockbackStateNeutral(this);
            KnockbackState.Subscribe();
        }

        public void Knockback(BattlerController against, int power)
        {
            KnockbackState.Knockback(against, power);
        }

        public abstract void Control();
    }
}