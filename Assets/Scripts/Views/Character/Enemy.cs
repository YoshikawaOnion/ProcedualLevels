using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;
using System;
using ProcedualLevels.Common;

namespace ProcedualLevels.Views
{
	public class Enemy : MonoBehaviour
	{
        [SerializeField]
        private float walkSpeed;

        private GameObject SearchArea { get; set; }
        private CompositeDisposable Disposable { get; set; }

        public void Initialize()
        {
            Disposable = new CompositeDisposable();
            SearchArea = transform.Find("SearchArea").gameObject;

            SearchArea.OnTriggerEnter2DAsObservable()
                      .Where(x => x.tag == Def.PlayerTag)
                      .Subscribe(x => RaiseFoundState(x))
                      .AddTo(Disposable);
        }

        private void RaiseFoundState(Collider2D collision)
        {
            Disposable.Dispose();
            Debug.Log("Found by Enemy!");
            var hero = collision.gameObject;
            var rigidbody = GetComponent<Rigidbody2D>();
            var direction = (hero.transform.position - transform.position).normalized * walkSpeed;

            this.UpdateAsObservable()
                .Subscribe(x => rigidbody.velocity = direction.MergeY(rigidbody.velocity.y));
        }
    }   
}
