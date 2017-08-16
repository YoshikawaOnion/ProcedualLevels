using System;
using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// ノックバックの情報を集約するクラス。
    /// </summary>
    public class KnockbackInfo
    {
        public Battler BattlerSubject { get; set; }
        public Battler BattlerAgainst { get; set; }
        public int Power { get; set; }
        public float KnockbackPower { get; set; }
        public float KnockbackJumpPower { get; set; }
        public float StanTime { get; set; }

        /// <summary>
        /// パラメーターを指定して、KnockbackInfoの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="subject">ノックバックを受けるバトラー。</param>
        /// <param name="against">ノックバックを発生させるバトラー。</param>
        /// <param name="noResist">ノックバックを受ける側が無抵抗に最大のノックバックを受けるかどうかを表す真偽値。</param>
        public KnockbackInfo(Battler subject, Battler against, bool noResist, int minPower = 0)
        {
            var asset = AssetRepository.I.GameParameterAsset;

            if (noResist)
            {
                Power = Mathf.Max(0, against.Attack.Value);
            }
            else
			{
				var knockbackPower = Mathf.Max(0, against.Attack.Value - subject.Attack.Value);
				Power = knockbackPower;
            }
            Power = Mathf.Max(minPower, Power);
			BattlerSubject = subject;
			BattlerAgainst = against;
			KnockbackPower = Power * asset.KnockbackPowerFactor;
			KnockbackJumpPower = asset.KnockbackJumpPower;
			StanTime = 1;
		}

        public KnockbackInfo Clone()
        {
            return new KnockbackInfo(BattlerSubject, BattlerAgainst, false)
            {
                KnockbackPower = KnockbackPower,
                KnockbackJumpPower = KnockbackJumpPower,
                StanTime = StanTime,
                Power = Power
            };
        }
    }
}