using UnityEngine;
using System.Collections;

namespace ProcedualLevels.Views
{
    public class PlayerContext : EventContext
    {
        public Player Owner { get; set; }
        public IGameEventReceiver GameEvents { get; set; }
    }
}