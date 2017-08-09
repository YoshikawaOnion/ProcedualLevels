using System;
using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class Spike
    {
        public Vector2 InitialPosition { get; set; }
        public int Index { get; set; }
        public int Attack { get; set; }

        public Spike(int index, IAdventureView view)
        {
            Index = index;
            Attack = AssetRepository.I.GameParameterAsset.SpikeDamage;
        }
    }
}