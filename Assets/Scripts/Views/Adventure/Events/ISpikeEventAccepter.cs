using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// トゲに関するイベントを受け付けるインターフェース。
    /// </summary>
    public interface ISpikeEventAccepter
    {
        IObserver<Tuple<SpikeController, BattlerController>> OnBattlerTouchedSpikeSender { get; }
    }
}