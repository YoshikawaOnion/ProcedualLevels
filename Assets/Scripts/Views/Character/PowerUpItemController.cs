using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Models;
using UnityEngine;
using UniRx;
using System;
using UniRx.Triggers;

namespace ProcedualLevels.Views
{
    public class PowerUpItemController : MonoBehaviour
    {
        public PowerUp Model { get; private set; }

        public void Initialize(PowerUp model, IPowerUpItemEventAccepter eventAccepter)
        {
            Model = model;

            var rigidbody = GetComponent<Rigidbody2D>();
            rigidbody.AddForce(new Vector2(0, 3));

            var collider = GetComponent<Collider2D>();
            collider.enabled = false;
            collider.OnTriggerEnter2DAsObservable()
                    .Where(x => x.tag == Def.PlayerTag)
                    .Subscribe(x =>
            {
                eventAccepter.OnPlayerGetPowerUpSender.OnNext(Model);
                this.UpdateAsObservable()
                    .Take(5)
                    .Select(y => (x.transform.position + transform.position * 2) / 3)
                    .Subscribe(y => transform.position = y, () => Destroy(gameObject));
            });

            Observable.Timer(TimeSpan.FromMilliseconds(500))
                      .Subscribe(x => collider.enabled = true);
        }
    }
}