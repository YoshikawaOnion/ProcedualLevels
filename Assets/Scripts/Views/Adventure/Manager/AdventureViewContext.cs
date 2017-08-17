using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using ProcedualLevels.Models;
using UnityEngine;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// 探索画面のコンテキスト クラス。
    /// </summary>
    public class AdventureViewContext
    {
        public IGameEventReceiver EventReceiver { get; set; }
        public HeroController Hero { get; set; }
        public AdventureContext Model { get; set; }
        public GameUiManager UiManager { get; set; }
        public GameObjectManager ObjectManager { get; set; }
    }
}