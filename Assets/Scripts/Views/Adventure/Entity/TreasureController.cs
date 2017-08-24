using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using ProcedualLevels.Models;
using UniRx;

namespace ProcedualLevels.Views
{
    public class TreasureController : MonoBehaviour
    {
        public void Initialize(Treasure treasure)
        {
            this.OnTriggerEnter2DAsObservable()
                .Where(x => !treasure.IsGotten && x.tag == Def.PlayerTag)
                .Subscribe(x =>
            {
                var sprite = GetComponentInChildren<Script_SpriteStudio_Root>();
                var index = sprite.IndexGetAnimation("Open");
                sprite.AnimationPlay(index, 1);
                treasure.OnPlayerTouch();
            })
                .AddTo(this);
        }
    }
}