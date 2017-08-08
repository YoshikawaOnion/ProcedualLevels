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

        private Text Text { get; set; }
        private CompositeDisposable Disposable { get; set; }
        private Color InitialColor { get; set; }

        private void Start()
		{
			Text = GetComponent<Text>();
			InitialColor = Text.color;
		}

        public void Initialize(Models.AdventureContext context)
        {
            Text.color = InitialColor;

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
                Text.text = stringBuilder.ToString();
            }).AddTo(Disposable);

            context.TimeLimit.FirstOrDefault(x => x == 0)
                   .Subscribe(x => Text.color = dangerColor)
                   .AddTo(Disposable);
            context.TimeLimit.FirstOrDefault(x => x <= 30)
                   .Subscribe(x => Text.color = warningColor)
                   .AddTo(Disposable);
        }

        private void OnDestroy()
        {
            Disposable.Dispose();
            Text = null;
        }
    }
}