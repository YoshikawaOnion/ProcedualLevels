using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Models;
using UnityEngine;
using UniRx.Triggers;
using UniRx;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// トゲに関する処理を提供するクラス。
    /// </summary>
    public class SpikeController : MonoBehaviour
    {
        public Spike Spike { get; set; }

        public void Initialize(Spike spike,
                               ISpikeEventAccepter eventAccepter)
        {
            Spike = spike;

            // キャラクターがぶつかったらイベントを発行
            gameObject.OnCollisionStay2DAsObservable()
                      .Select(x => x.gameObject.GetComponent<BattlerController>())
                      .Where(x => x != null)
                      .Subscribe(x =>
            {
                eventAccepter.OnBattlerTouchedSpikeSender
                             .OnNext(Tuple.Create(this, x));
            });
        }
    }
}