using UnityEngine;
using System.Collections;
using UniRx;
using System;

namespace ProcedualLevels.Models
{
    public class Hero : Battler
    {
        public Hero(int index, IAdventureView view)
            : base(index, view)
        {
            view.BattleObservable.Subscribe(x =>
            {
                Hp.Value -= x.Attack.Value;
                x.Hp.Value -= Attack.Value;
                view.Knockback(this, x, x.Attack.Value);
                view.Knockback(x, this, Attack.Value);
            });
            Observable.Interval(TimeSpan.FromMilliseconds(500))
                      .Skip(2)
                      .TakeUntil(view.BattleObservable)
                      .Repeat()
                      .Subscribe(x =>
            {
                Hp.Value += 1;
            });
        }
    }
}