using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UniRx.Triggers;

namespace ProcedualLevels.Views
{
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

        public void Push()
        {
            IsHold = true;
            onPushed.OnNext(Unit.Default);
        }

        public void Release()
        {
            IsHold = false;
        }
    }
}