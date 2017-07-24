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

		public Battler()
		{
			Attack = new ReactiveProperty<int>();
            Range = new ReactiveProperty<int>();
			Hp = new ReactiveProperty<int>();
			MaxHp = new ReactiveProperty<int>();
			Hp.Where(x => x > MaxHp.Value || x < 0)
			  .Subscribe(x => Hp.Value = Helper.Clamp(Hp.Value, 0, MaxHp.Value));
		}
    }
}