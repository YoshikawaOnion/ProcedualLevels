using System;

namespace ProcedualLevels.Models
{
    public class AdventureContext
    {
        public Hero Hero { get; set; }
        public MapData Map { get; set; }
        public Enemy[] Enemies { get; set; }

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
