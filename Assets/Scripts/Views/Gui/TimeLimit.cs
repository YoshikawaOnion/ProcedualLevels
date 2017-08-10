using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Text;
using UnityEngine.UI;
using System;

namespace ProcedualLevels.Views
{
    public class TimeLimit : MonoBehaviour
    {
        [SerializeField]
        private Color dangerColor = Color.white;
        [SerializeField]
        private Color warningColor = Color.white;

        private CompositeDisposable Disposable { get; set; }
        private Color InitialColor { get; set; }
        private UILabel Label { get; set; }

        private void Awake()
        {
            Label = GetComponent<UILabel>();
            InitialColor = Label.color;
        }

        public void Initialize(Models.AdventureContext context)
        {
            Label.color = InitialColor;

            if (Disposable != null)
            {
                Disposable.Dispose();
            }
            Disposable = new CompositeDisposable();

            context.TimeLimit.Subscribe(x =>
            {
                var stringBuilder = new StringBuilder();
                var minutes = x / 60;
                var seconds = x % 60;
                stringBuilder.Append(minutes.ToString("00"));
                stringBuilder.Append(":");
                stringBuilder.Append(seconds.ToString("00"));
                Label.text = stringBuilder.ToString();
            }).AddTo(Disposable);

            context.TimeLimit.FirstOrDefault(x => x == 0)
                   .Subscribe(x => Label.color = dangerColor)
                   .AddTo(Disposable);
            context.TimeLimit.FirstOrDefault(x => x <= 30)
                   .Subscribe(x => Label.color = warningColor)
                   .AddTo(Disposable);
        }

        private void OnDestroy()
        {
            Disposable.Dispose();
            Label = null;
        }
    }
}