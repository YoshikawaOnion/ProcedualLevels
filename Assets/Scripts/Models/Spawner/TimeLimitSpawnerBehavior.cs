using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class TimeLimitSpawnerBehavior : ISpawnerBehavior
    {
        public IObservable<Unit> GetSpawnStream(AdventureContext context)
		{
			return context.TimeLimit.First(x => x <= 0)
                          .SelectMany(Observable.Interval(TimeSpan.FromMinutes(1)))
                          .Select(x => Unit.Default);
        }
    }
}