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
        public int Id { get; set; }
        public string PrefabName { get; set; }
        public int Hp { get; set; }
        public int Attack { get; set; }
        /// <summary>
        /// 生成アルゴリズムを提供するクラスのインスタンスを取得または設定します。
        /// </summary>
        public EnemyGenStrategy GenerationStrategy { get; set; }
    }
}