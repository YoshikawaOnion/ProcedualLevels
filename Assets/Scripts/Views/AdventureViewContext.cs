using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Models;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public class AdventureViewContext
    {
        public IGameEventReceiver EventReceiver { get; set; }
        public HeroController Hero { get; set; }
        public AdventureContext Model { get; set; }
    }
}