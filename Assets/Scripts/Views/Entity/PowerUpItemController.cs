using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Models;
using UnityEngine;
using UniRx;
using System;
using UniRx.Triggers;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// パワーアップアイテムの挙動を管理するクラス。
    /// </summary>
    public class PowerUpItemController : MonoBehaviour
    {
        /// <summary>
        /// モデル側のオブジェクトを取得します。
        /// </summary>
        /// <value>The model.</value>
        public PowerUp Model { get; private set; }
        private CompositeDisposable Disposable { get; set; }

        /// <summary>
        /// パワーアップアイテムの挙動を初期化します。
        /// </summary>
        /// <param name="model">モデル側のインスタンス。</param>
        /// <param name="eventAccepter">このインスタンスからのイベントの送信口。</param>
        public void Initialize(PowerUp model, IPowerUpItemEventAccepter eventAccepter)
        {
            Model = model;
            Disposable = new CompositeDisposable();

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
                    .Subscribe(y => transform.position = y, () => Destroy(gameObject))
                    .AddTo(Disposable);
            })
                    .AddTo(Disposable);

            Observable.Timer(TimeSpan.FromMilliseconds(500))
                      .Subscribe(x => collider.enabled = true)
                      .AddTo(Disposable);
        }

        private void OnDestroy()
        {
            Disposable.Dispose();
        }
    }
}