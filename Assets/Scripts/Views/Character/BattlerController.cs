using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace ProcedualLevels.Views
{
    public class BattlerController : MonoBehaviour
    {
        public void Initialize(Models.Battler battler)
		{
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
        }
    }
}