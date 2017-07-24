using UnityEngine;
using System.Collections;
using UniRx;

namespace ProcedualLevels.Models
{
    public class Battler
	{
		public ReactiveProperty<int> Attack { get; private set; }
        public ReactiveProperty<int> Range { get; private set; }
		public ReactiveProperty<int> Hp { get; private set; }
		public ReactiveProperty<int> MaxHp { get; private set; }

        private IAdventureView view;

		public Battler(IAdventureView view)
		{
            this.view = view;

			Attack = new ReactiveProperty<int>(1);
            Range = new ReactiveProperty<int>(1);
			Hp = new ReactiveProperty<int>(10);
			MaxHp = new ReactiveProperty<int>(10);
			Hp.Where(x => x > MaxHp.Value || x < 0)
			  .Subscribe(x => Hp.Value = Helper.Clamp(Hp.Value, 0, MaxHp.Value));
		}
    }
}