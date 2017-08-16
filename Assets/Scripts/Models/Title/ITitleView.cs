using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public interface ITitleView
    {
        IObservable<Unit> OnTap { get; }

        IObservable<IAdventureView> GotoAdventureAsync();
    }
}