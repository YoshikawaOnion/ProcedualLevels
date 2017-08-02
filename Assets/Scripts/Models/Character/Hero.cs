using UnityEngine;
using System.Collections;
using UniRx;
using System;

namespace ProcedualLevels.Models
{
    public class Hero : Battler
    {
        private CompositeDisposable Disposable { get; set; }

        public Hero(int index, IAdventureView view)
            : base(index, view)
        {
            Disposable = new CompositeDisposable();

            view.BattleObservable.Subscribe(x =>
            {
                Hp.Value -= x.Attack.Value;
                x.Hp.Value -= Attack.Value;
                view.Knockback(this, x, x.Attack.Value);
                view.Knockback(x, this, Attack.Value);
            })
                .AddTo(Disposable);
            
            view.GetPowerUpObservable.Subscribe(x =>
            {
                Attack.Value += x.AttackRising;
                x.Gotten();
            })
                .AddTo(Disposable);

            Observable.Interval(TimeSpan.FromMilliseconds(500))
                      .Skip(2)
                      .TakeUntil(view.BattleObservable)
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