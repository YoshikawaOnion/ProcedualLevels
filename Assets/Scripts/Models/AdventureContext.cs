using System;

namespace ProcedualLevels.Models
{
    public class AdventureContext
    {
        public Hero Hero { get; set; }
        public MapData Map { get; set; }
        public Enemy[] Enemeis { get; set; }
    }
}
