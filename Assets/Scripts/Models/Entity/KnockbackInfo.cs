﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// ノックバックの情報を集約するクラス。
    /// </summary>
    public class KnockbackInfo
    {
        public Battler BattlerSubject { get; private set; }
        public Battler BattlerAgainst { get; private set; }
        public int Power { get; private set; }
        public float KnockbackPower { get; private set; }
        public float KnockbackJumpPower { get; private set; }
        public float StanTime { get; private set; }

        /// <summary>
        /// パラメーターを指定して、KnockbackInfoの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="subject">ノックバックを受けるバトラー。</param>
        /// <param name="against">ノックバックを発生させるバトラー。</param>
        /// <param name="noResist">ノックバックを受ける側が無抵抗に最大のノックバックを受けるかどうかを表す真偽値。</param>
        public KnockbackInfo(Battler subject, Battler against, bool noResist)
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
			BattlerSubject = subject;
			BattlerAgainst = against;
			KnockbackPower = Power * asset.KnockbackPowerFactor;
			KnockbackJumpPower = asset.KnockbackJumpPower;
			StanTime = 1 * asset.KnockbackStanTimeFactor;
		}
    }
}