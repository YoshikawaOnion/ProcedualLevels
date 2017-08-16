using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public interface IGoalEventAccepter
    {
        IObserver<Unit> OnPlayerGoalSender { get; }
    }
}
