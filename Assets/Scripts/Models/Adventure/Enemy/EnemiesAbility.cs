using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// 敵キャラクターのパラメータ。
    /// </summary>
    public class EnemiesAbility
    {
        /// <summary>
        /// この敵キャラクターパラメータのID。
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }
        /// <summary>
        /// この敵キャラクターを制御するプレハブの名前。
        /// </summary>
        public string PrefabName { get; set; }
        /// <summary>
        /// この敵キャラクターの最大HP。
        /// </summary>
        /// <value>The hp.</value>
        public int Hp { get; set; }
        /// <summary>
        /// この敵キャラクターの攻撃力。
        /// </summary>
        public int Attack { get; set; }
        /// <summary>
        /// 生成アルゴリズムを提供するクラスのインスタンスを取得または設定します。
        /// </summary>
        public EnemyGenStrategy GenerationStrategy { get; set; }
    }
}