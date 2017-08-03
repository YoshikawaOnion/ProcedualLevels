using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public interface ISpawnerBehavior
    {
        IObservable<Unit> GetSpawnStream(AdventureContext context);
    }
}