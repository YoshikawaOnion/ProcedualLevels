using UnityEngine;
using System.Collections;

namespace ProcedualLevels.Views
{
    public class HeroContext
    {
        public HeroController Hero { get; set; }
        public IGameEventReceiver GameEvents { get; set; }
    }
}