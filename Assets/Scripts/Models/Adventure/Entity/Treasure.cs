using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class Treasure
    {
        private AdventureContext Context { get; set; }
        public bool IsGotten { get; private set; }
        public Vector2 InitialLocation { get; private set; }

        public Treasure(Vector2 initialLocation, AdventureContext context)
        {
            Context = context;
            IsGotten = false;
            InitialLocation = initialLocation;
        }

        public void OnPlayerTouch()
        {
            if (IsGotten)
            {
                return;
            }

            Context.Score.Value += AssetRepository.I.GameParameterAsset.ScoreForTreasure;
            IsGotten = true;
        }
    }
}