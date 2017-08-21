using System;
using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Models;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public class TitleManager : MonoBehaviour, ITitleView
    {
        private Subject<Unit> onTapSubject;

        public IObservable<Unit> OnTap { get; private set; }

        private void Start()
        {
            onTapSubject = new Subject<Unit>();
            OnTap = onTapSubject;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                onTapSubject.OnNext(Unit.Default);
                onTapSubject.OnCompleted();
            }
        }

        public IObservable<IAdventureView> GotoAdventureAsync()
        {
            return SceneHelper.ChangeToRootScene();
        }
    }
}