using UnityEngine;
using System.Collections;

namespace ProcedualLevels.Models
{
    public class Enemy : Battler
    {
        public Vector2 InitialPosition { get; private set; }

        public Enemy(int index, Vector2 initialPos, IAdventureView view)
            : base(index, view)
        {
            InitialPosition = initialPos;
        }
    }
}