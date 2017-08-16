using UnityEngine;
using System.Collections;
using UniRx;
using ProcedualLevels.Common;
using System;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// 戦闘に参加できるキャラクターを表す基底クラス。
    /// </summary>
    public abstract class Battler
	{
        public int Index { get; set; }
		public ReactiveProperty<int> Attack { get; private set; }
        public ReactiveProperty<int> Range { get; private set; }
		public ReactiveProperty<int> Hp { get; private set; }
		public ReactiveProperty<int> MaxHp { get; private set; }
        public ReactiveProperty<bool> IsAlive { get; private set; }

        private IDisposable Disposable { get; set; }

		public Battler(int index, IAdventureView view)
		{
            this.Index = index;

			Attack = new ReactiveProperty<int>(1);
            Range = new ReactiveProperty<int>(1);
			Hp = new ReactiveProperty<int>(10);
			MaxHp = new ReactiveProperty<int>(10);
            IsAlive = new ReactiveProperty<bool>(true);

			Hp.Where(x => x > MaxHp.Value || x < 0)
			  .Subscribe(x => 
            {
                Hp.Value = Helper.Clamp(Hp.Value, 0, MaxHp.Value);
            });
            Hp.Subscribe(x => IsAlive.Value = x > 0);
            IsAlive.Where(x => !x)
                   .Subscribe(x => view.ShowDeath(this));

            Disposable = view.OnBattlerTouchSpike
                             .Where(x => x.Item2.Index == Index)
                             .ThrottleFirst(TimeSpan.FromMilliseconds(750))
                             .Subscribe(x =>
            {
                Hp.Value -= x.Item1.Attack;
            });
		}

        public virtual void Dispose()
        {
            Disposable.Dispose();
        }
    }
}