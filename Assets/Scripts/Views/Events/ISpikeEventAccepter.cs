using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public interface ISpikeEventAccepter
    {
        IObserver<Tuple<SpikeController, BattlerController>> OnBattlerTouchedSpikeSender { get; }
    }
}