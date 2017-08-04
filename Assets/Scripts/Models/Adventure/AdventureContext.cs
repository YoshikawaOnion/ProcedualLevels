using System;
using System.Collections.Generic;
using UniRx;

namespace ProcedualLevels.Models
{
    public class AdventureContext
    {
        public Hero Hero { get; set; }
        public MapData Map { get; set; }
		public List<Enemy> Enemies { get; set; }
		public ReactiveProperty<int> TimeLimit { get; set; }
        public int NextBattlerIndex { get; set; }
        public IAdventureView View { get; set; }
        public Spawner[] Spawners { get; set; }

        public AdventureContext()
        {
            TimeLimit = new ReactiveProperty<int>();
            Enemies = new List<Enemy>();
        }

        public void SpawnEnemy(Enemy enemy)
        {
            Enemies.Add(enemy);
            View.SpawnEnemy(enemy);
        }

        public void Dispose()
        {
            Hero.Dispose();
            foreach (var enemy in Enemies)
            {
                enemy.Dispose();
            }
            foreach (var spawner in Spawners)
            {
                spawner.Dispose();
            }
        }
    }
}
