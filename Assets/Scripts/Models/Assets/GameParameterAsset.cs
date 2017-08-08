using UnityEngine;
using System.Collections;

namespace ProcedualLevels.Models
{
    public class GameParameterAsset : ScriptableObject
    {
        public int PlayerHp;
        public int PlayerAttack;
        public float KnockbackPowerFactor;
        public float KnockbackJumpPower;
        public float KnockbackStanTimeFactor;
        public int TimeLimitSeconds;
        public int SpikeDamage;
    }
}
