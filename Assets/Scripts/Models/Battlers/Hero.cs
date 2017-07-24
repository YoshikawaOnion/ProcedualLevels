using UnityEngine;
using System.Collections;
using UniRx;

namespace ProcedualLevels.Models
{
    public class Hero : Battler
    {
        public Hero(IAdventureView view)
            : base(view)
        {
            view.BattleObservable.Subscribe(x =>
            {
                Hp.Value -= x.Attack.Value;
                x.Hp.Value -= Attack.Value;
            });
        }
    }
}