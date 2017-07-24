using UnityEngine;
using System.Collections;

namespace ProcedualLevels.Models
{
    public interface IAdventureView
    {
        void Initialize(AdventureContext context);
    }
}