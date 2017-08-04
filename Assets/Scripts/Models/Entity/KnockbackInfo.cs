using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class KnockbackInfo
    {
        public Battler BattlerSubject { get; private set; }
        public Battler BattlerAgainst { get; private set; }
        public int Power { get; private set; }
        public float KnockbackPower { get; private set; }
        public float KnockbackJumpPower { get; private set; }
        public float StanTime { get; private set; }

        public KnockbackInfo(Battler subject, Battler against, GameParameterAsset asset)
        {
            var knockbackPower = Mathf.Max(0, against.Attack.Value - subject.Attack.Value);
            BattlerSubject = subject;
            BattlerAgainst = against;
            Power = knockbackPower;
            KnockbackPower = Power * asset.KnockbackPowerFactor;
            KnockbackJumpPower = asset.KnockbackJumpPower;
            StanTime = 1 * asset.KnockbackStanTimeFactor;
        }
    }
}