using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using DG.Tweening;
using ProcedualLevels.Common;

namespace ProcedualLevels.Views
{
    public class Charge : MonoBehaviour
    {
        public void Initialize(Player parent, int index)
        {
            parent.Charges[index].Subscribe(x => gameObject.SetActive(x));

            Observable.EveryUpdate()
                      .Subscribe(x =>
            {
                var angleBase = index * (360.0f / parent.chargeMax);
                var angle = angleBase + x * 2;
                transform.localPosition = Vector2Extensions.FromAngleLength(angle, 16)
                    .ToVector3();
            });
        }
    }
}