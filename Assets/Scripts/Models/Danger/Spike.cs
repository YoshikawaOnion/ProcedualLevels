using System;
using System.Collections;
using System.Collections.Generic;
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
            Attack = 3;

            view.OnBattlerTouchSpike
                .Where(x => x.Item1.Index == Index)
                .ThrottleFirst(TimeSpan.FromMilliseconds(750))
                .Subscribe(x =>
            {
                x.Item2.Hp.Value -= Attack;
            });
        }
    }
}