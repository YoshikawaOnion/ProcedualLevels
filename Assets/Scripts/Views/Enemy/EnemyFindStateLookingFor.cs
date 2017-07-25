using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;

namespace ProcedualLevels.Views
{
    public class EnemyFindStateLookingFor : EnemyFindState
    {
        public EnemyFindStateLookingFor(EnemyController context) : base(context)
        {
        }

        public override void Subscribe()
        {
            base.Subscribe();
            var searchArea = Context.transform.Find("SearchArea").gameObject;
            searchArea.OnTriggerEnter2DAsObservable()
                      .Subscribe(x => OnSearch(x))
                      .AddTo(Disposable);
        }

        private void OnSearch(Collider2D collider)
		{
			var hero = collider.GetComponent<HeroController>();
            if (hero != null)
            {
				ChangeState(new EnemyFindStateFound(Context, hero));
            }
        }

        public override void Control()
        {
        }
    }
}