using System;
using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Models;
using UniRx;
using UnityEngine;
using UniRx.Triggers;

namespace ProcedualLevels.Views
{
    public class GameOverController : MonoBehaviour, Models.IGameOverView
    {
        public IObservable<Unit> OnTap { get; private set; }

        public void Initialize()
        {
            OnTap = this.UpdateAsObservable()
                        .Where(x => Input.GetMouseButtonDown(0));
        }

        public IObservable<IAdventureView> GotoAdventure()
        {
            Destroy(gameObject);

            var viewPrefab = Resources.Load<Views.GameManager>("Prefabs/Manager/GameManager");
            IAdventureView view = Instantiate(viewPrefab);

            return Observable.NextFrame()
                             .Select(x => view);
        }
    }
}