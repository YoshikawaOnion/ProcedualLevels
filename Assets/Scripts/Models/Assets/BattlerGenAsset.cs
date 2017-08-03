using UnityEngine;
using System.Collections;

namespace ProcedualLevels.Models
{
    public class BattlerGenAsset : ScriptableObject
    {
        public int PlayerHp;
        public int PlayerAttack;
        public float KnockbackPowerFactor;
        public float KnockbackJumpPower;
        public float KnockbackStanTimeFactor;
    }
}
