﻿using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UniRx.Triggers;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// nGUIに対応したボタンの管理クラス。
    /// </summary>
    public class ControllerButton : MonoBehaviour
    {
        private Subject<Unit> onPushed;

        public bool IsHold { get; set; }
        public IObservable<Unit> OnPushed
        {
            get { return onPushed; }
        }

        public ControllerButton()
        {
            onPushed = new Subject<Unit>();
        }

        void OnPress(bool isDown)
        {
            if (isDown)
            {
                IsHold = true;
                onPushed.OnNext(Unit.Default);
            }
            else
            {
                IsHold = false;
            }
        }
    }
}