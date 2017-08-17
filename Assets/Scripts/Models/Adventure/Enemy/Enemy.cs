using UnityEngine;
using System.Collections;
using UniRx;
using System;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// 敵キャラクターを表すクラス。
    /// </summary>
    public class Enemy : Battler
    {
        /// <summary>
        /// この敵キャラクターの初期位置。
        /// </summary>
        public Vector2 InitialPosition { get; private set; }
        /// <summary>
        /// この敵キャラクターを倒すとアイテムをドロップするかどうかを表す真偽値。
        /// </summary>
        public bool DropPowerUp { get; set; }
        /// <summary>
        /// この敵キャラクターのパラメータ。
        /// </summary>
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
					  .Skip(3)
					  .TakeUntil(view.OnBattle.Where(x => x.Index == Index))
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

        public override void Dispose()
        {
            base.Dispose();
            Disposable.Dispose();
        }
    }
}