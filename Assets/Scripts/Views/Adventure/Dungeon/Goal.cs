using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// ゴール地点オブジェクトの処理を行うクラス。
    /// </summary>
    public class Goal : MonoBehaviour
    {
        private AdventureViewContext Context { get; set; }
        private IGoalEventAccepter EventAccepter { get; set; }

        public void Initialize(AdventureViewContext context, IGoalEventAccepter eventAccepter)
        {
            EventAccepter = eventAccepter;
            Context = context;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == Def.PlayerTag)
            {
                EventAccepter.OnPlayerGoalSender.OnNext(UniRx.Unit.Default);
                Context.UiManager.ClearText.SetActive(true);
            }
        }
    }
}