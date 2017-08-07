using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class TimeSpanSpawnerBehavior : ISpawnerBehavior
    {
        private TimeSpan TimeSpan;

        public TimeSpanSpawnerBehavior(TimeSpan timeSpan)
        {
            TimeSpan = timeSpan;
        }

        public IObservable<Unit> GetSpawnStream(AdventureContext context)
        {
            return Observable.Interval(TimeSpan)
                             .Select(x => Unit.Default);
        }
    }
}