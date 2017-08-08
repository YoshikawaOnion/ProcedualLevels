using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Models;
using UnityEngine;
using UniRx.Triggers;
using UniRx;

namespace ProcedualLevels.Views
{
    public class SpikeController : MonoBehaviour
    {
        public void Initialize(Spike spike,
                               ISpikeEventAccepter eventAccepter)
        {
            gameObject.OnCollisionStay2DAsObservable()
                      .Select(x => x.gameObject.GetComponent<BattlerController>())
                      .Where(x => x != null)
                      .Subscribe(x =>
            {
                eventAccepter.OnBattlerTouchedSpikeSender
                             .OnNext(Tuple.Create(spike, x.Battler));
            });
        }
    }
}