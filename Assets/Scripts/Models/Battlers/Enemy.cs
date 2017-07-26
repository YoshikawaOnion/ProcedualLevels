using UnityEngine;
using System.Collections;
using UniRx;
using System;

namespace ProcedualLevels.Models
{
    public class Enemy : Battler
    {
        public Vector2 InitialPosition { get; private set; }

        public Enemy(int index, Vector2 initialPos, IAdventureView view)
            : base(index, view)
        {
            InitialPosition = initialPos;

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