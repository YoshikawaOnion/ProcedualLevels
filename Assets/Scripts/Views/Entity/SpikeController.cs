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
        public Spike Spike { get; set; }

        public void Initialize(Spike spike,
                               ISpikeEventAccepter eventAccepter)
        {
            gameObject.OnCollisionStay2DAsObservable()
                      .Select(x => x.gameObject.GetComponent<BattlerController>())
                      .Where(x => x != null)
                      .Subscribe(x =>
            {
                Spike = spike;

                eventAccepter.OnBattlerTouchedSpikeSender
                             .OnNext(Tuple.Create(this, x));
            });
        }
    }
}