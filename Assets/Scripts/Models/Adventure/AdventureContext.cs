using System;
using UniRx;

namespace ProcedualLevels.Models
{
    public class AdventureContext
    {
        public Hero Hero { get; set; }
        public MapData Map { get; set; }
		public Enemy[] Enemies { get; set; }
		public ReactiveProperty<int> TimeLimit { get; set; }

        public AdventureContext()
        {
            TimeLimit = new ReactiveProperty<int>();
        }

        public void Dispose()
        {
            Hero.Dispose();
            foreach (var enemy in Enemies)
            {
                enemy.Dispose();
            }
        }
    }
}
