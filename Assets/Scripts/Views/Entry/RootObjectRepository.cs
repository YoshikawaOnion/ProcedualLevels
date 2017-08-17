using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public class RootObjectRepository : Singleton<RootObjectRepository>
    {
        public Camera Camera;
        public Script_SpriteStudio_ManagerDraw ManagerDraw;
        public GameObject GameUi;

        protected override void Init()
        {
        }
    }
}