using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public interface IGameOverView
    {
        IObservable<Unit> OnTap { get; }

        IObservable<IAdventureView> GotoAdventure();
    }
}