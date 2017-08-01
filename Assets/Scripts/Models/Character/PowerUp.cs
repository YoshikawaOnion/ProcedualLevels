using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class PowerUp
    {
        public Vector2 InitialPosition { get; set; }
        public int AttackRising { get; set; }
        public IObservable<Unit> OnGotten
        {
            get { return onGottenSubject; }
        }

        private Subject<Unit> onGottenSubject;

        public PowerUp()
        {
            onGottenSubject = new Subject<Unit>();
        }

        public void Gotten()
        {
            onGottenSubject.OnNext(Unit.Default);
        }
    }
}