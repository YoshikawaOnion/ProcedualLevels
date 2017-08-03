using UnityEngine;
using System.Collections;
using UniRx;
using System;

namespace ProcedualLevels.Models
{
    public class Hero : Battler
    {
        private CompositeDisposable Disposable { get; set; }
        private GameParameterAsset BattlerAsset { get; set; }

        public Hero(int index, IAdventureView view)
            : base(index, view)
        {
            BattlerAsset = Resources.Load<GameParameterAsset>(Def.GameParameterAssetPath);
            Disposable = new CompositeDisposable();

            view.OnBattle.Subscribe(x =>
            {
                Hp.Value -= x.Attack.Value;
                x.Hp.Value -= Attack.Value;
                var knockbackForThis = new KnockbackInfo(this, x, BattlerAsset);
                var knockbackForEnemy = new KnockbackInfo(x, this, BattlerAsset);
                view.Knockback(knockbackForThis);
                view.Knockback(knockbackForEnemy);
                Debug.Log(string.Format("{0} vs {1}", Attack.Value, x.Attack.Value));
            })
                .AddTo(Disposable);
            
            view.OnGetPowerUp.Subscribe(x =>
            {
                Attack.Value += x.AttackRising;
                x.Gotten();
            })
                .AddTo(Disposable);

            Observable.Interval(TimeSpan.FromMilliseconds(500))
                      .Skip(2)
                      .TakeUntil(view.OnBattle)
                      .Repeat()
                      .Subscribe(x =>
            {
                Hp.Value += 1;
            })
                      .AddTo(Disposable);
        }

        public void Dispose()
        {
            Disposable.Dispose();
        }
    }
}