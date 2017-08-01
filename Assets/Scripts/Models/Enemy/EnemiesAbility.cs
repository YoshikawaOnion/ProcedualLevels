using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class EnemiesAbility
    {
        public int Id { get; set; }
        public string PrefabName { get; set; }
        public int Hp { get; set; }
        public int Attack { get; set; }
        public EnemyGenStrategy GenerationStrategy { get; set; }
    }
}