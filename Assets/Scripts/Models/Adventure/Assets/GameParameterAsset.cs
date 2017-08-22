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
        [Tooltip("ノックバックの連鎖ごとのスタン時間増加量")]
        public float KnockbackChainStanTimeDelta;
        public int TimeLimitSeconds;
        public int SpikeDamage;
        public int SpikeNumber;
        public float TrampleForce;
        public float RecoverTimeFromTrampled;
        [Tooltip("プレイヤーの吹き飛ばし力の最小値")]
        public int PlayerKnockbackPowerBase;
    }
}
