using System;
using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// トゲを表すクラス。
    /// </summary>
    public class Spike
    {
        /// <summary>
        /// トゲの初期位置を取得または設定します。
        /// </summary>
        public Vector2 InitialPosition { get; set; }
        /// <summary>
        /// トゲの一意なインデックスを取得します。
        /// </summary>
        public int Index { get; private set; }
        /// <summary>
        /// このトゲに当たった際のダメージ量を取得します。
        /// </summary>
        /// <value>The attack.</value>
        public int Attack { get; private set; }

        public Spike(int index, IAdventureView view)
        {
            Index = index;
            Attack = AssetRepository.I.GameParameterAsset.SpikeDamage;
        }
    }
}