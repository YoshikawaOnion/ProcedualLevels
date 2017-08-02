using UnityEngine;
using System.Collections;
using UniRx;
using System;

namespace ProcedualLevels.Models
{
    public class Enemy : Battler
    {
        public Vector2 InitialPosition { get; private set; }
        public bool DropPowerUp { get; set; }
        public EnemiesAbility Ability { get; set; }

        private CompositeDisposable Disposable { get; set; }

        public Enemy(int index, Vector2 initialPos, EnemiesAbility ability, IAdventureView view)
            : base(index, view)
        {
            Disposable = new CompositeDisposable();
            InitialPosition = initialPos;
            MaxHp.Value = ability.Hp;
            Hp.Value = ability.Hp;
            Attack.Value = ability.Attack;
            Ability = ability;

			Observable.Interval(TimeSpan.FromMilliseconds(500))
					  .Skip(2)
					  .TakeUntil(view.BattleObservable)
					  .Repeat()
					  .Subscribe(x =>
			{
				Hp.Value += 1;
			})
                      .AddTo(Disposable);

            IsAlive.Where(x => !x && DropPowerUp)
                   .FirstOrDefault()
                   .Subscribe(x => view.PlacePowerUp(index, new PowerUp() { AttackRising = 1 } ))
                   .AddTo(Disposable);
        }

        public void Dispose()
        {
            Disposable.Dispose();
        }
    }
}