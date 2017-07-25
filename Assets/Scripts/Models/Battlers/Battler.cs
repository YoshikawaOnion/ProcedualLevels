using UnityEngine;
using System.Collections;
using UniRx;

namespace ProcedualLevels.Models
{
    public class Battler
	{
        public int Index { get; set; }
		public ReactiveProperty<int> Attack { get; private set; }
        public ReactiveProperty<int> Range { get; private set; }
		public ReactiveProperty<int> Hp { get; private set; }
		public ReactiveProperty<int> MaxHp { get; private set; }
        public ReactiveProperty<bool> IsAlive { get; private set; }

        private IAdventureView view;

		public Battler(int index, IAdventureView view)
		{
            this.Index = index;
            this.view = view;

			Attack = new ReactiveProperty<int>(1);
            Range = new ReactiveProperty<int>(1);
			Hp = new ReactiveProperty<int>(10);
			MaxHp = new ReactiveProperty<int>(10);
            IsAlive = new ReactiveProperty<bool>(true);

			Hp.Where(x => x > MaxHp.Value || x < 0)
			  .Subscribe(x => 
            {
                Hp.Value = Helper.Clamp(Hp.Value, 0, MaxHp.Value);
                IsAlive.Value = Hp.Value > 0;
            });
            IsAlive.Where(x => !x)
                   .Subscribe(x => view.ShowDeath(this));
		}
    }
}