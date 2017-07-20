using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace ProcedualLevels.Views
{
    public class Copy : MonoBehaviour
    {
        [SerializeField]
        private Vector2 ballUv;
        [SerializeField]
        private Vector2 dropUv;

        public Vector2 Uv { get; private set; }
        public IObservable<Unit> OnVanish { get; private set; }

        private Subject<Unit> onVanish_ { get; set; }

        public void Initialize()
		{
			Observable.Timer(TimeSpan.FromSeconds(60))
					  .Subscribe(x => Reset());
			Uv = dropUv;
			onVanish_ = new Subject<Unit>();
			OnVanish = onVanish_;
            gameObject.SetActive(true);
        }

        public void Reset()
        {
            gameObject.SetActive(false);
            onVanish_.OnNext(Unit.Default);
            onVanish_.OnCompleted();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Uv = ballUv;
        }
    }
}