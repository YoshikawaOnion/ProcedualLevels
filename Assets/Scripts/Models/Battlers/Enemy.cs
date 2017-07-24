using UnityEngine;
using System.Collections;

namespace ProcedualLevels.Models
{
    public class Enemy : Battler
    {
        public Vector2 InitialPosition { get; private set; }

        public Enemy(Vector2 initialPos, IAdventureView view)
            : base(view)
        {
            InitialPosition = initialPos;
        }
    }
}