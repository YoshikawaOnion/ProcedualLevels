using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// ゴール地点に関するイベントを受け付けるインターフェース。
    /// </summary>
    public interface IGoalEventAccepter
    {
        IObserver<Unit> OnPlayerGoalSender { get; }
    }
}
