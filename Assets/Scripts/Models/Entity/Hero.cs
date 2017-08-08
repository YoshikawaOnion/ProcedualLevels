using UnityEngine;
using System.Collections;
using UniRx;
using System;
using ProcedualLevels.Common;

namespace ProcedualLevels.Models
{
    public class Hero : Battler
    {
        private CompositeDisposable Disposable { get; set; }

        public Hero(int index, IAdventureView view)
            : base(index, view)
        {
            Disposable = new CompositeDisposable();

            view.OnBattle.Subscribe(x =>
            {
                Hp.Value -= x.Attack.Value;
                x.Hp.Value -= Attack.Value;
                var knockbackForThis = new KnockbackInfo(this, x, false);
                var knockbackForEnemy = new KnockbackInfo(x, this, false);
                view.Knockback(knockbackForThis);
                view.Knockback(knockbackForEnemy);
            })
                .AddTo(Disposable);
            
            view.OnAttacked.Subscribe(x =>
            {
                Hp.Value -= x.Attack.Value;
                var knockbackForThis = new KnockbackInfo(this, x, true);
                view.Knockback(knockbackForThis);
            })
                .AddTo(Disposable);
            
            view.OnGetPowerUp.Subscribe(x =>
            {
                Attack.Value += x.AttackRising;
                x.Gotten();
            })
                .AddTo(Disposable);

            // 一定時間攻撃を受けないでいるとHPが回復していく
            Observable.Interval(TimeSpan.FromMilliseconds(250))
                      .Skip(2)
                      .TakeUntil(view.OnBattle)
                      .TakeUntil(view.OnAttacked)
                      .TakeUntil(view.OnBattlerTouchSpike.Where(x => x.Item2.Index == index))
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