using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public class Goal : MonoBehaviour
    {
        private IGoalEventAccepter EventAccepter { get; set; }

        public void Initialize(IGoalEventAccepter eventAccepter)
        {
            EventAccepter = eventAccepter;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == Def.PlayerTag)
            {
                EventAccepter.OnPlayerGoalSender.OnNext(UniRx.Unit.Default);
                var clearText = GameObject.Find("Canvas/GameUi/ClearText");
                clearText.SetActive(true);
            }
        }
    }
}