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
                var knockbackForThis = new KnockbackInfo(this, x, BattlerAsset, false);
                var knockbackForEnemy = new KnockbackInfo(x, this, BattlerAsset, false);
                view.Knockback(knockbackForThis);
                view.Knockback(knockbackForEnemy);
            })
                .AddTo(Disposable);

            view.OnAttacked.Subscribe(x =>
            {
                Hp.Value -= x.Attack.Value;
                var knockbackForThis = new KnockbackInfo(this, x, BattlerAsset, true);
                view.Knockback(knockbackForThis);
            })
                .AddTo(Disposable);
            
            view.OnGetPowerUp.Subscribe(x =>
            {
                Attack.Value += x.AttackRising;
                x.Gotten();
            })
                .AddTo(Disposable);

            Observable.Interval(TimeSpan.FromMilliseconds(250))
                      .Skip(2)
                      .TakeUntil(view.OnBattle)
                      .TakeUntil(view.OnAttacked)
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